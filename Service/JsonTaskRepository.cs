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
    var array = ((MyArray<TaskItem>) tasks).ToArray();
    string json = JsonSerializer.Serialize(array,
      new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(_filePath, json);
  }
}