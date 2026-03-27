public class TaskService : ITaskService
{
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
    return CanModify(user, task.Assignee);
  }
  public void AddTask(string description, PriorityLevel priority, string assignee)
  { 
    int maxId = _tasks.Reduce(0, (max, t) => t.Id > max ? t.Id : max);
    var newTask = new TaskItem {
        Id = maxId + 1,
        Description = description,
        Completed = false,
        Priority = priority,
        CreatedAt = DateTime.Now,
        Assignee = assignee
      }; 
    _tasks.Add(newTask); 
    _repository.SaveTasks(_tasks);
  }
  public void UpdateTask(int id, string description, string user)
  {
    var task = _tasks.FindBy(id, (t, id) => t.Id.CompareTo(id));
    if (task != null)
    {
      task.Description = description;
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
  public void ToggleTaskCompletion(int id, string user)
  {
    var task = _tasks.FindBy(id, (t, k) => t.Id.CompareTo(k));
    if (task != null)
    {
      task.Completed = !task.Completed;
      _repository.SaveTasks(_tasks);
    }
  }
  public IMyCollection<TaskItem> FilterTasks(IMyCollection<TaskItem> tasks, Func<TaskItem, bool> predicate)
  {
    return tasks.Filter(predicate);
  }
}