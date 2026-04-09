public class TaskItem : IComparable<TaskItem>
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public bool Completed { get; set; }
    public PriorityLevel Priority { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string Assignee { get; set; } = null!;

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
