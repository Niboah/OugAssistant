
using OugAssistant.Features.Planning.Model;
using OugAssistant_APP.DTO.Planning;

namespace OugAssistant_WEB.Models.Planning
{
    public class TaskViewModel
    {
        public IEnumerable<TaskAPIout> TaskList { get; set; }
        public IEnumerable<GoalAPIout> GoalList { get; set; }
        public HashSet<TimeOnly>[] WeekTimes { get; set; }

        public TaskViewModel() { }
    }
}

