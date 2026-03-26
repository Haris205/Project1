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
    var tasks = JsonSerializer.Deserialize<TaskItem[]>(json); 
    if (tasks != null)
    {
      foreach (var task in tasks)
      {
        result.Add(task);
      }
    }
    return result;
  }
  public void SaveTasks(IMyCollection<TaskItem> tasks)
  {
    var taskList = new MyLinkedList<TaskItem>();
    foreach (var task in tasks)
    {
      taskList.Add(task);
    }

    string json = JsonSerializer.Serialize(taskList.ToArray(),
      new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(_filePath, json);
  }
}