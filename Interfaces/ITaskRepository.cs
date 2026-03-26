public interface ITaskRepository { 
    IMyCollection<TaskItem> LoadTasks(CollectionType type); 
    void SaveTasks(IMyCollection<TaskItem> tasks); 
}