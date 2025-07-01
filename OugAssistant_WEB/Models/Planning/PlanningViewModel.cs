
using OugAssistant.Features.Planning.Model;
using OugAssistant_APP.DTO.Planning;

namespace OugAssistant_WEB.Models.Planning
{
    public class PlanningViewModel
    {
        public IEnumerable<TaskAPIout> TaskList { get; set; }
        public IEnumerable<GoalAPIout> GoalList { get; set; }

        public PlanningViewModel() { }
    }
}

