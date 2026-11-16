public class TaskService : ITaskService
{

  /** 
   * 1. An unassigned task can be updated by anyone
   * 2. A way/option to change the current user in the application to another user
   * 3. Display current user somewhere in the application
   * 4. Implement task dependency
   * 5. Studeer jouw code
  */
  private readonly ITaskRepository _repository;
  private IMyCollection<TaskItem> _tasks = null!;
  private CollectionType _collectionType;
  public TaskService(ITaskRepository repository, CollectionType type)
  {
    _repository = repository;
    SetCollectionType(type);
  }
  public IMyCollection<TaskItem> GetTaskAsCollection() => _tasks;
  public void SetCollectionType(CollectionType type)
  {
    _collectionType = type;

    _tasks = CollectionFactory.Create<TaskItem>(type);
    var loaded = _repository.LoadTasks(type);
    foreach (var task in loaded)
    {
      _tasks.Add(task);
    }
  }
  public bool TaskExists(int id)
  {
      return _tasks.FindBy(id, (t, k) => t.Id.CompareTo(k)) != null;
  }
  public IMyCollection<TaskItem> GetAllTasks()
  {
    var result = CollectionFactory.Create<TaskItem>(_collectionType);
    var iterator = _tasks.GetIterator();

    while (iterator.HasNext())
    {
      result.Add(iterator.Next());
    }

    return result;
  }
  bool CanModify(string? user, string? assignee)
  {
    return !string.IsNullOrWhiteSpace(user)
      && string.Equals(user.Trim(), assignee?.Trim(), StringComparison.OrdinalIgnoreCase);
  }
  public bool CanUserModify(int id, string user)
  {
    var task = _tasks.FindBy(id, (t, id) => t.Id.CompareTo(id));
    if (task == null) return false;
    return string.Equals(task.Assignee?.Trim(), "none", StringComparison.OrdinalIgnoreCase)
      || CanModify(user, task.Assignee);
  }
  public void AddTask(string description, PriorityLevel priority, string assignee, MyLinkedList<int> dependencyIds)
  {
      int maxId = _tasks.Reduce(0, (max, t) => t.Id > max ? t.Id : max);

      var newTask = new TaskItem
      {
          Id = maxId + 1,
          Description = description,
          Status = Status.ToDo,
          Priority = priority,
          CreatedAt = DateTime.Now,
          Assignee = assignee,
          Dependencies = dependencyIds
      };

      _tasks.Add(newTask);
      _repository.SaveTasks(_tasks);
  }
  public void UpdateTask(int id, string description, string assignee, string user, PriorityLevel? priority = null)
  {
    var task = _tasks.FindBy(id, (t, id) => t.Id.CompareTo(id));
    if (task != null)
    {
      if (!CanUserModify(id, user))
      {
        throw new UnauthorizedAccessException("You cannot modify this task.");
      }

      task.Description = description;
      task.Assignee = string.IsNullOrWhiteSpace(assignee) ? "none" : assignee;
      if (priority.HasValue) task.Priority = priority.Value;
      _repository.SaveTasks(_tasks);
    }
  }
  public void RemoveTask(int id, string user)
  {
    var task = _tasks.FindBy(id, (t, id) => t.Id.CompareTo(id));
    if (task != null)
    {
      if (!CanUserModify(id, user))
      {
        throw new UnauthorizedAccessException("You cannot modify this task.");
      }

      _tasks.Remove(task);
      _repository.SaveTasks(_tasks);
    }
  }
  public void UpdateDependencies(int taskId, MyLinkedList<int> dependencyIds, string user)
  {
      var task = _tasks.FindBy(taskId, (t, id) => t.Id.CompareTo(id));
      if (task == null) return;

      if (!CanUserModify(taskId, user))
      {
        throw new UnauthorizedAccessException("You cannot modify this task.");
      }

      var validDeps = new MyLinkedList<int>();
      var seen = new HashSet<int>();
      var it = dependencyIds.GetIterator();
      while (it.HasNext())
      {
          int depId = it.Next();
          if (depId == taskId || !TaskExists(depId) || !seen.Add(depId))
          {
              throw new InvalidOperationException($"Dependency {depId} is invalid.");
          }

          if (DependsOn(depId, taskId, new HashSet<int>()))
          {
              throw new InvalidOperationException("The dependency would create a cycle.");
          }

          validDeps.Add(depId);
      }

      task.Dependencies = validDeps;

      _repository.SaveTasks(_tasks);
  }

  private bool DependsOn(int taskId, int dependencyId, HashSet<int> visited)
  {
      if (taskId == dependencyId) return true;
      if (!visited.Add(taskId)) return false;

      var task = _tasks.FindBy(taskId, (item, id) => item.Id.CompareTo(id));
      if (task?.Dependencies == null) return false;

      var dependencies = task.Dependencies.GetIterator();
      while (dependencies.HasNext())
      {
          if (DependsOn(dependencies.Next(), dependencyId, visited))
          {
              return true;
          }
      }

      return false;
  }
  public bool AreDependenciesDone(int taskId)
  {
      var task = _tasks.FindBy(taskId, (t, k) => t.Id.CompareTo(k));
      if (task == null) return true;

      var it = task.Dependencies.GetIterator();

      while (it.HasNext())
      {
          int depId = it.Next();

          var depTask = _tasks.FindBy(depId, (t, k) => t.Id.CompareTo(k));

          if (depTask == null || depTask.Status != Status.Done)
              return false;
      }

      return true;
  }
  public void ToggleTaskState(int id, string user, Status status)
  {
      var task = _tasks.FindBy(id, (t, k) => t.Id.CompareTo(k));
      if (task == null) return;

      if (!CanUserModify(id, user))
      {
          throw new UnauthorizedAccessException("You cannot modify this task.");
      }

      if (status == Status.Done && !AreDependenciesDone(id))
      {
          throw new InvalidOperationException("Complete this task's dependencies first.");
      }

      task.Status = status;

      _repository.SaveTasks(_tasks);
  }
  public IMyCollection<TaskItem> FilterTasks(IMyCollection<TaskItem> tasks, Func<TaskItem, bool> predicate)
  {
    return tasks.Filter(predicate);
  }
}