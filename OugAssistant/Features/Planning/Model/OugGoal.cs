using System.ComponentModel.DataAnnotations;

namespace OugAssistant.Features.Planning.Model;

public class OugGoal
{
    [Key]
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<OugTask> Tasks;
    public bool Archived = false;

    public OugGoal(string name, string description)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Tasks = new List<OugTask>();
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

    public bool CheckGoal()
    {
        if (Tasks.All(x => x.FinishDateTime != null))
        {
            Archived = true;
        }
        return Archived;
    }
}

