namespace OugAssistant.Features.Planning.Model;

public class Mission: Task
{
    public DateTime DeadLine { get; set; }
    public Mission() { }
    public Mission(string name, string description, TaskPriority priority, DateTime deadline)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Priority = priority;
        DeadLine = deadline;
    }
}

