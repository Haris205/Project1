using System.Text.Json;

public class JsonTaskRepository : ITaskRepository { 
  private readonly string _filePath; 
  public JsonTaskRepository(string filePath) => _filePath = filePath; 
 
  public IMyCollection<TaskItem> LoadTasks(CollectionType type) {
    var result = CollectionFactory.Create<TaskItem>(type);
    if (!File.Exists(_filePath)) { 
      return result;
    } 
    string json = File.ReadAllText(_filePath); 
    var tasks = JsonSerializer.Deserialize<StoredTask[]>(json);
    if (tasks != null)
    {
      foreach (var task in tasks)
      {
        var item = new TaskItem
        {
          Id = task.Id,
          Description = task.Description,
          Priority = task.Priority,
          CreatedAt = task.CreatedAt,
          Assignee = task.Assignee ?? "none",
          Status = task.Status,
          Dependencies = new MyLinkedList<int>()
        };
        foreach (var dependencyId in task.Dependencies ?? [])
        {
          item.Dependencies.Add(dependencyId);
        }
        result.Add(item);
      }
    }
    return result;
  }
  public void SaveTasks(IMyCollection<TaskItem> tasks)
  {
    var taskList = new List<StoredTask>();
    foreach (var task in tasks)
    {
      taskList.Add(new StoredTask
      {
        Id = task.Id,
        Description = task.Description,
        Priority = task.Priority,
        CreatedAt = task.CreatedAt,
        Assignee = task.Assignee,
        Status = task.Status,
        Dependencies = task.Dependencies?.ToArray() ?? []
      });
    }

    string json = JsonSerializer.Serialize(taskList,
      new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(_filePath, json);
  }

  private sealed class StoredTask
  {
    public StoredTask() { }

    public int Id { get; set; }
    public string Description { get; set; } = "";
    public PriorityLevel Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Assignee { get; set; }
    public Status Status { get; set; }
    public int[]? Dependencies { get; set; }
  }
}