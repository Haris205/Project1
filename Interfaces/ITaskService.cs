public interface ITaskService { 
  IEnumerable<TaskItem> GetAllTasks(); 
  void AddTask(string description);
  void UpdateTask(int id, string description);
  void RemoveTask(int id); 
  void ToggleTaskCompletion(int id); 
}