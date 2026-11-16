using Microsoft.AspNetCore.Http;

public static class TaskEndpoints
{
  public static void MapTaskEndpoints(this IEndpointRouteBuilder endpoints)
  {
    var tasks = endpoints.MapGroup("/api/tasks");
    tasks.MapGet("", GetTasks);
    tasks.MapPost("", CreateTask);
    tasks.MapPut("/{id:int}", UpdateTask);
    tasks.MapDelete("/{id:int}", DeleteTask);
    tasks.MapPatch("/{id:int}/status", UpdateStatus);
    tasks.MapPut("/{id:int}/dependencies", UpdateDependencies);
  }

  private static IResult GetTasks(ITaskService service)
  {
    lock (service)
    {
      return Results.Ok(GetTaskResponses(service));
    }
  }

  private static IResult CreateTask(CreateTaskRequest request, HttpContext context, ITaskService service)
  {
    var user = GetCurrentUser(context);
    if (user is null) return Problem(400, "A current user is required.");
    if (string.IsNullOrWhiteSpace(request.Description))
      return Problem(400, "Task description cannot be empty.");
    if (!Enum.IsDefined(request.Priority)) return Problem(400, "Unknown task priority.");

    lock (service)
    {
      var dependencies = request.Dependencies ?? [];
      if (!DependenciesExist(service, dependencies))
        return Problem(400, "Each dependency must reference an existing task.");

      var nextId = GetTaskResponses(service).Select(task => task.Id).DefaultIfEmpty(0).Max() + 1;
      var dependencyList = ToLinkedList(dependencies);
      service.AddTask(request.Description.Trim(), request.Priority, NormalizeAssignee(request.Assignee), dependencyList);
      var created = FindTask(service, nextId);
      return created is null
        ? Problem(500, "The task was saved but could not be loaded.")
        : Results.Created($"/api/tasks/{created.Id}", created);
    }
  }

  private static IResult UpdateTask(int id, UpdateTaskRequest request, HttpContext context, ITaskService service)
  {
    var user = GetCurrentUser(context);
    if (user is null) return Problem(400, "A current user is required.");
    if (string.IsNullOrWhiteSpace(request.Description))
      return Problem(400, "Task description cannot be empty.");

    lock (service)
    {
      if (!service.TaskExists(id)) return Results.NotFound();
      if (!service.CanUserModify(id, user)) return Problem(403, "You cannot modify this task.");
      if (request.Priority.HasValue && !Enum.IsDefined(request.Priority.Value))
        return Problem(400, "Unknown task priority.");

      if (request.Dependencies is not null)
      {
        var dependencies = request.Dependencies;
        if (dependencies.Contains(id) || dependencies.Distinct().Count() != dependencies.Length)
          return Problem(400, "Dependencies must be unique and cannot include the task itself.");
        if (!DependenciesExist(service, dependencies))
          return Problem(400, "Each dependency must reference an existing task.");
        if (WouldCreateCycle(service, id, dependencies))
          return Problem(400, "The dependency would create a cycle.");
        service.UpdateDependencies(id, ToLinkedList(dependencies), user);
      }

      service.UpdateTask(id, request.Description.Trim(), NormalizeAssignee(request.Assignee), user, request.Priority);
      return Results.Ok(FindTask(service, id));
    }
  }

  private static IResult DeleteTask(int id, HttpContext context, ITaskService service)
  {
    var user = GetCurrentUser(context);
    if (user is null) return Problem(400, "A current user is required.");

    lock (service)
    {
      if (!service.TaskExists(id)) return Results.NotFound();
      if (!service.CanUserModify(id, user)) return Problem(403, "You cannot modify this task.");

      if (GetTaskResponses(service).Any(task => task.Id != id && task.Dependencies.Contains(id)))
        return Problem(409, "Remove this task from its dependents before deleting it.");

      service.RemoveTask(id, user);
      return Results.NoContent();
    }
  }

  private static IResult UpdateStatus(int id, UpdateStatusRequest request, HttpContext context, ITaskService service)
  {
    var user = GetCurrentUser(context);
    if (user is null) return Problem(400, "A current user is required.");
    if (!Enum.IsDefined(request.Status)) return Problem(400, "Unknown task status.");

    lock (service)
    {
      if (!service.TaskExists(id)) return Results.NotFound();
      if (!service.CanUserModify(id, user)) return Problem(403, "You cannot modify this task.");
      if (request.Status == Status.Done && !service.AreDependenciesDone(id))
        return Problem(409, "Complete this task's dependencies before marking it done.");

      service.ToggleTaskState(id, user, request.Status);
      return Results.Ok(FindTask(service, id));
    }
  }

  private static IResult UpdateDependencies(
    int id,
    UpdateDependenciesRequest request,
    HttpContext context,
    ITaskService service)
  {
    var user = GetCurrentUser(context);
    if (user is null) return Problem(400, "A current user is required.");

    lock (service)
    {
      if (!service.TaskExists(id)) return Results.NotFound();
      if (!service.CanUserModify(id, user)) return Problem(403, "You cannot modify this task.");

      var dependencies = request.DependencyIds ?? [];
      if (dependencies.Contains(id) || dependencies.Distinct().Count() != dependencies.Length)
        return Problem(400, "Dependencies must be unique and cannot include the task itself.");
      if (!DependenciesExist(service, dependencies))
        return Problem(400, "Each dependency must reference an existing task.");
      if (WouldCreateCycle(service, id, dependencies))
        return Problem(400, "The dependency would create a cycle.");

      service.UpdateDependencies(id, ToLinkedList(dependencies), user);
      return Results.Ok(FindTask(service, id));
    }
  }

  private static string? GetCurrentUser(HttpContext context)
  {
    var user = context.Request.Headers["X-Current-User"].ToString().Trim();
    return string.IsNullOrWhiteSpace(user) ? null : user;
  }

  private static string NormalizeAssignee(string? assignee) =>
    string.IsNullOrWhiteSpace(assignee) || assignee.Trim().Equals("none", StringComparison.OrdinalIgnoreCase)
      ? "none"
      : assignee.Trim();

  private static IResult Problem(int status, string message) =>
    Results.Problem(statusCode: status, title: message);

  private static MyLinkedList<int> ToLinkedList(IEnumerable<int> ids)
  {
    var result = new MyLinkedList<int>();
    foreach (var id in ids) result.Add(id);
    return result;
  }

  private static List<TaskResponse> GetTaskResponses(ITaskService service)
  {
    var result = new List<TaskResponse>();
    foreach (var task in service.GetAllTasks()) result.Add(ToResponse(task));
    return result;
  }

  private static TaskResponse? FindTask(ITaskService service, int id)
  {
    foreach (var task in service.GetAllTasks())
    {
      if (task.Id == id) return ToResponse(task);
    }
    return null;
  }

  private static TaskResponse ToResponse(TaskItem task) =>
    new(task.Id, task.Description, task.Priority, task.CreatedAt, task.Assignee, task.Status,
      task.Dependencies?.ToArray() ?? []);

  private static bool DependenciesExist(ITaskService service, IEnumerable<int> ids)
  {
    foreach (var id in ids)
    {
      if (!service.TaskExists(id)) return false;
    }
    return true;
  }

  private static bool WouldCreateCycle(ITaskService service, int taskId, IEnumerable<int> dependencies)
  {
    var tasks = GetTaskResponses(service).ToDictionary(task => task.Id);
    var pending = new Stack<int>(dependencies);
    var visited = new HashSet<int>();

    while (pending.TryPop(out var currentId))
    {
      if (currentId == taskId) return true;
      if (!visited.Add(currentId) || !tasks.TryGetValue(currentId, out var current)) continue;
      foreach (var dependencyId in current.Dependencies) pending.Push(dependencyId);
    }

    return false;
  }
}
