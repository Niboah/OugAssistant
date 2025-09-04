namespace OugAssistant.Features.Planning.Model;

public class OugEvent : OugTask
{

    public DateTime EventDateTime { get; set; }
    public string Place { get; set; }

    public OugEvent() { }
    public OugEvent(string name, string? description, TaskPriority priority, OugTask? parentTask, ICollection<OugGoal> goalList, DateTime eventDateTime, string place) : base(name, description, priority, parentTask, goalList)
    {
        EventDateTime = eventDateTime;
        Place = place;
    }

}

