using System.ComponentModel.DataAnnotations;

namespace OugAssistant.Features.Planning.Model;

public enum TaskPriority
{
    LOW, MEDIUM, HIGH
}

public abstract class OugTask
{
    [Key]
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime? FinishDateTime { get; private set; }

    // FK - Goal
    public Guid GoalId { get; set; }
    public OugGoal Goal { get; set; }

    public OugTask() { }
    public OugTask(string name, string? description, TaskPriority priority, Guid goalId, OugGoal goal)
    {
        Id = new Guid();
        Name = name;
        Description = description;
        Priority = priority;
        GoalId = goalId;
        Goal = goal;
        Goal.AddTask(this);

    }

    public bool Finish()
    {
        FinishDateTime = DateTime.Now;
        return Goal.CheckGoal();
    }
}

