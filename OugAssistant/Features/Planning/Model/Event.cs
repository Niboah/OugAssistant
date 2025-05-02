namespace OugAssistant.Features.Planning.Model;

public class Event : Task
{
    public DateTime DateTime { get; set; }
    public string Place {  get; set; }
    public Event() { }
    public Event (string name,string description,TaskPriority priority,DateTime dateTime, string place)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Priority = priority;
        DateTime = dateTime;
        Place = place;
    }
}

