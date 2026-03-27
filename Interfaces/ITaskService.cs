public interface ITaskService { 
  IMyCollection<TaskItem> GetAllTasks();
  IMyCollection<TaskItem> GetTaskAsCollection();
  void SetCollectionType(CollectionType type);
  void AddTask(string description, PriorityLevel priority, string assignee);
  void UpdateTask(int id, string description, string user);
  void RemoveTask(int id, string user); 
  void ToggleTaskCompletion(int id, string user);
  bool CanUserModify(int id, string user);
  IMyCollection<TaskItem> FilterTasks(IMyCollection<TaskItem> tasks, Func<TaskItem, bool> predicate);
}