namespace OugAssistant.Features.Planning.Model;

public class OugMission : OugTask
{
    public DateTime DeadLine { get; set; }
    public OugMission() { }
    public OugMission(string name, string? description, TaskPriority priority, Guid goalId, OugGoal goal, DateTime deadLine) : base(name, description, priority, goalId, goal)
    {
        DeadLine = deadLine;
    }

}
