public class TaskService : ITaskService
{
  private readonly ITaskRepository _repository;
  private readonly IMyCollection<TaskItem> _tasks;
  public TaskService(ITaskRepository repository)
  {
    _repository = repository;
    var loaded = _repository.LoadTasks();
    _tasks = new MyArray<TaskItem>();

    foreach (var task in loaded)
    {
      _tasks.Add(task);
    }
  }
  public IEnumerable<TaskItem> GetAllTasks()
  {
    var result = new MyArray<TaskItem>();
    var iterator = _tasks.GetIterator();

    while (iterator.HasNext())
    {
      result.Add(iterator.Next());
    }

    return result.ToArray();
  }
  public void AddTask(string description, PriorityLevel priority)
  { 
    int maxId = _tasks.Reduce(0, (max, t) => t.Id > max ? t.Id : max);
    var newTask = new TaskItem { Id = maxId + 1, Description = description, Completed = false, Priority = priority, CreatedAt = DateTime.Now}; 
    _tasks.Add(newTask); 
    _repository.SaveTasks(_tasks);
  }
  public void UpdateTask(int id, string description)
  {
    var task = _tasks.FindBy(id, (t, id) => t.Id.CompareTo(id));
    if (task != null)
    {
      task.Description = description;
      _repository.SaveTasks(_tasks);
    }
  }
  public void RemoveTask(int id)
  {
    var task = _tasks.FindBy(id, (t, id) => t.Id.CompareTo(id));
    if (task != null)
    {
      _tasks.Remove(task);
      _repository.SaveTasks(_tasks);
    }
  }
  public void ToggleTaskCompletion(int id)
  {
    var task = _tasks.FindBy(id, (t, k) => t.Id.CompareTo(k));
    if (task != null)
    {
      task.Completed = !task.Completed;
      _repository.SaveTasks(_tasks);
    }
  }
}