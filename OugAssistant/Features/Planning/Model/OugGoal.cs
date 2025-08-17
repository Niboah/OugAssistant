using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace OugAssistant.Features.Planning.Model;

public class OugGoal
{
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Archived { get; set; } = false;
    public int Level { get; protected set; }
    public ICollection<OugTask> Tasks { get; set; } = new List<OugTask>();
    public OugGoal? Parent { get; set; }
    public ICollection<OugGoal> Childs { get; protected set; } = new List<OugGoal>();

    public OugGoal() { }
    public OugGoal(string name, string description, OugGoal? parentGoal = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;

        if (parentGoal == null)
        {
            Level = 1;
            Parent = null;
        }
        else
        {
            Parent = parentGoal;
            Parent.Childs.Add(this);
            Level = parentGoal.Level + 1;
        }
    }

    public void AddTask(OugTask task)
    {
        if (!Tasks.Any(t => t.Id == task.Id)) {
            Tasks.Add(task);
            if (Archived) Archived = false;
        }
    }

    public bool ArchiveGoal()
    {
        if (getCompletion() == 1.0m)
        {
            Archived = true;
        }
        return Archived;
    }

    public decimal getCompletion()
    {
        int total = 0;
        int completed = 0;
        foreach (var task in Tasks)
        {
            total += 1;
            if (task.FinishedDateTime != null) completed += 1;
        }
        foreach (var childGoal in Childs)
        {
            total += 1;
            if (childGoal.Archived || childGoal.getCompletion() == 1.0m) completed += 1;
        }
        return completed / (decimal)total;
    }
}

