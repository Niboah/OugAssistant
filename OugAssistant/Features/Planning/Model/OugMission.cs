namespace OugAssistant.Features.Planning.Model;

public class OugMission : OugTask
{
    public DateTime DeadLine { get; set; }
    public OugMission() { }
    public OugMission(string name, string? description, TaskPriority priority, OugTask? parentTask, ICollection<OugGoal> goalList, DateTime deadLine) : base(name, description, priority, parentTask, goalList)
    {
        DeadLine = deadLine;
    }

}
