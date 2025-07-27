using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace OugAssistant.Features.Planning.Model;

public class OugGoal
{
    [Key]
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Archived = false;
    public ICollection<OugTask> Tasks;
    public OugGoal? ParentGoal { get; set; }
    public int Level { get; protected set; }
    public ICollection<OugGoal> ChildGoals { get; protected set; }  = new List<OugGoal>();

    public OugGoal() { }
    public OugGoal(string name, string description, OugGoal? parentGoal = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Tasks = new List<OugTask>();
        ChildGoals = new List<OugGoal>();

        if (parentGoal == null)
        {
            Level = 0;
            ParentGoal = null;
        }
        else
        {
            ParentGoal = parentGoal;
            ParentGoal.ChildGoals.Add(this);
            Level = parentGoal.Level + 1;
        }
    }

    public void AddTask(OugTask task)
    {
        Tasks.Add(task);
        if (Archived) Archived = false;
    }

    public ICollection<OugTask> getTasks()
    {
        return Tasks;
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
            if (task.FinishDateTime != null) completed += 1;
        }
        foreach (var childGoal in ChildGoals)
        {
            total += 1;
            if (childGoal.Archived || childGoal.getCompletion() == 1.0m) completed += 1;
        }
        return completed / (decimal)total;
    }
}

