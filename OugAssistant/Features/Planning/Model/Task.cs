namespace OugAssistant.Features.Planning.Model;

public enum TaskPriority
{
    LOW, MEDIUM, HIGH
}

public abstract class Task
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
}

