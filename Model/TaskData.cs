public class TaskItem : IComparable<TaskItem>
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string Assignee { get; set; } = null!;
    public Status Status { get; set; } = Status.ToDo;
    public MyLinkedList<int> Dependencies = new();
    public int CompareTo(TaskItem? other)
    {
        return other == null ? 1 : Priority.CompareTo(other.Priority);
    }
}
public enum PriorityLevel
{
    Low,
    Medium,
    High
}

public enum Status
{
    ToDo,
    InProgress,
    Done
}
