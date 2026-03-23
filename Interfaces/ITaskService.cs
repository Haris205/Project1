public interface ITaskService { 
  IMyCollection<TaskItem> GetAllTasks();
  IMyCollection<TaskItem> GetTaskAsCollection();
  void SetCollectionType(CollectionType type);
  void AddTask(string description, PriorityLevel priority);
  void UpdateTask(int id, string description);
  void RemoveTask(int id); 
  void ToggleTaskCompletion(int id);
  IMyCollection<TaskItem> FilterTasks(IMyCollection<TaskItem> tasks, Func<TaskItem, bool> predicate);
}