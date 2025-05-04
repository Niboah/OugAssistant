
using OugAssistant.Features.Planning.Model;

namespace OugAssistant_WEB.Models.Planning
{
    public class TaskViewModel
    {
        public List<OugTask> TaskList { get; set; }
        public List<OugGoal> GoalList { get; set; }
        public List<OugRoutine> RoutineList { get; set; }
    }
}

