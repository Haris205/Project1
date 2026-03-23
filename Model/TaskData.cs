public class TaskItem
{
    public int Id { get; set; }
    public required string Description { get; set; }
    public bool Completed { get; set; }
    public PriorityLevel Priority { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
public enum PriorityLevel
{
    Low,
    Medium,
    High
}
