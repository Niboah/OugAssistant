namespace OugAssistant.Features.Planning.Model;

public class OugEvent : OugTask
{

    public DateTime EventDateTime { get; set; }
    public string Place { get; set; }

    public OugEvent() { }
    public OugEvent(string name, string? description, TaskPriority priority, Guid goalId, OugGoal goal, DateTime eventDateTime, string place) : base(name, description, priority, goalId, goal)
    {
        EventDateTime = eventDateTime;
        Place = place;
    }

}

