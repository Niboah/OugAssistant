using System.ComponentModel.DataAnnotations;

namespace OugAssistant.Features.Planning.Model;

public enum TaskPriority
{
    LOW, MEDIUM, HIGH
}

public abstract class OugTask
{
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime CreatedDateTime { get; private set; } = DateTime.Now;
    public DateTime? FinishedDateTime { get; private set; }
    public ICollection<OugGoal> Goals { get; set; } = new List<OugGoal>();
    public OugTask? Parent { get; set; }
    public ICollection<OugTask> Childs { get; set; } = new List<OugTask>();

    public OugTask() { }
    public OugTask(string name, string? description, TaskPriority priority, OugTask? parentTask, ICollection<OugGoal> goalList)
    {
        Id = new Guid();
        Name = name;
        Description = description;
        Priority = priority;

        if (parentTask != null)
        {
            Parent = parentTask;
        }

        Goals = goalList;

        foreach (var item in Goals)
        {
            item.AddTask(this);
        }
    }

    public virtual bool Finish()
    {
        FinishedDateTime = DateTime.Now;

        bool result = false;

        foreach (var goal in Goals)
        {
            result = goal.ArchiveGoal() || result;
        }

        return result;
    }
}

