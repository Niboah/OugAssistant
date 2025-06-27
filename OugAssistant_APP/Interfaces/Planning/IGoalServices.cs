using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OugAssistant_APP.DTO.Planning;

namespace OugAssistant_APP.Interfaces.Planning
{
    public interface IGoalServices
    {
        Task<IEnumerable<GoalAPIout>> GetAllOugGoalAsync();
        Task<GoalAPIout?> GetOugGoalByIdAsync(Guid id);
        Task<bool> AddOugGoalAsync(GoalAPIin goal);
        Task<bool> UpdateOugGoalAsync(GoalAPIin goal);
        Task<bool> DeleteOugGoalAsync(Guid id);
        Task<bool> FinishGoal(Guid id);
    }
}
