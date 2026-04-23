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
  bool CanModify(string user, string assignee)
  {
    return user == assignee;
  }
  public bool CanUserModify(int id, string user)
  {
    var task = _tasks.FindBy(id, (t, id) => t.Id.CompareTo(id));
    if (task == null) return false;
    return task.Assignee == "none" ? true : CanModify(user, task.Assignee);
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
  public void UpdateTask(int id, string description, string assignee, string user)
  {
    var task = _tasks.FindBy(id, (t, id) => t.Id.CompareTo(id));
    if (task != null)
    {
      task.Description = description;
      task.Assignee = string.IsNullOrWhiteSpace(assignee) ? "none" : assignee;
      _repository.SaveTasks(_tasks);
    }
  }
  public void RemoveTask(int id, string user)
  {
    var task = _tasks.FindBy(id, (t, id) => t.Id.CompareTo(id));
    if (task != null)
    {
      _tasks.Remove(task);
      _repository.SaveTasks(_tasks);
    }
  }
  public void UpdateDependencies(int taskId, MyLinkedList<int> dependencyIds)
  {
      var task = _tasks.FindBy(taskId, (t, id) => t.Id.CompareTo(id));
      if (task == null) return;

      var validDeps = new MyLinkedList<int>();

      var it = dependencyIds.GetIterator();
      while (it.HasNext())
      {
          int depId = it.Next();

          var exists = _tasks.FindBy(depId, (t, id) => t.Id.CompareTo(id)) != null;

          if (exists && depId != taskId)
          {
              validDeps.Add(depId);
          }
      }

      task.Dependencies = validDeps;

      _repository.SaveTasks(_tasks);
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

      task.Status = status;

      _repository.SaveTasks(_tasks);
  }
  public IMyCollection<TaskItem> FilterTasks(IMyCollection<TaskItem> tasks, Func<TaskItem, bool> predicate)
  {
    return tasks.Filter(predicate);
  }
}