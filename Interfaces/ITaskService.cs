public interface ITaskService { 
  IMyCollection<TaskItem> GetAllTasks();
  IMyCollection<TaskItem> GetTaskAsCollection();
  void SetCollectionType(CollectionType type);
  void AddTask(string description, PriorityLevel priority, string assignee, MyLinkedList<int> dependencyId);
  void UpdateTask(int id, string description, string assignee, string user, PriorityLevel? priority = null);
  void RemoveTask(int id, string user);
  bool AreDependenciesDone(int taskId);
  void ToggleTaskState(int id, string user, Status status);
  bool CanUserModify(int id, string user);
  void UpdateDependencies(int taskId, MyLinkedList<int> dependencyIds, string user);
  bool TaskExists(int id);
  IMyCollection<TaskItem> FilterTasks(IMyCollection<TaskItem> tasks, Func<TaskItem, bool> predicate);
}