using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OugAssistant.Features.Planning.Model;

namespace OugAssistant_APP.Interfaces.IPlanningBD
{
    public interface IPlanningDB
    {
        #region Task
        Task<IEnumerable<OugTask>> GetAllOugTaskAsync(Type? type = null);
        Task<OugTask?> GetOugTaskByIdAsync(Guid id, Type? type = null);
        Task<bool> AddOugTaskAsync(OugTask item);
        Task<bool> UpdateOugTaskAsync(OugTask item);
        Task<bool> DeleteOugTaskAsync(Guid id);
        #endregion

        #region Goal
        Task<IEnumerable<OugGoal>> GetAllOugGoalAsync();
        Task<OugGoal?> GetOugGoalByIdAsync(Guid id);
        Task<bool> AddOugGoalAsync(OugGoal item);
        Task<bool> UpdateOugGoalAsync(OugGoal item);
        Task<bool> DeleteOugGoalAsync(Guid id);
        #endregion
    }
}
