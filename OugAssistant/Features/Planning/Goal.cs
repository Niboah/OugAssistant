
namespace OugAssistant.Features.Planning;

public class Goal
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Motivation { get; set; }

    private bool Archived=false;

    public Goal(Guid id, string name, string description, string motivation)
    {
        Id = id;
        Name = name;
        Description = description;
        Motivation = motivation;
    }

    public void Archive() { 
        Archived=true;
    }
}

