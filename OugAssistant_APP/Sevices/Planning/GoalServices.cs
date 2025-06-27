using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OugAssistant.Features.Planning.Model;
using OugAssistant_APP.DTO.Planning;
using OugAssistant_APP.Interfaces.IPlanningBD;
using OugAssistant_APP.Interfaces.Planning;

namespace OugAssistant_APP.Sevices.Planning
{
    public class GoalServices : IGoalServices
    {
        private readonly IPlanningDB _db;

        public GoalServices(IPlanningDB db)
        {
            this._db = db;
        }
        public async Task<IEnumerable<GoalAPIout>> GetAllOugGoalAsync()
        {
            var goals = await this._db.GetAllOugGoalAsync();
            return goals.OrderByDescending(g => g.Level).Select(g=> new GoalAPIout(g));
        }

        public async Task<GoalAPIout?> GetOugGoalByIdAsync(Guid id)
        {
            var goal = await this._db.GetOugGoalByIdAsync(id);
            return new GoalAPIout(goal);
        }

        public async Task<bool> AddOugGoalAsync(GoalAPIin goalIn)
        {
            
            OugGoal? parentGoal = null;
            if (goalIn.ParentGoalId != null) {
                parentGoal = await this._db.GetOugGoalByIdAsync((Guid)goalIn.ParentGoalId);
            }
            OugGoal goal = new OugGoal(goalIn.Name, goalIn.Description, parentGoal);
            return await this._db.AddOugGoalAsync(goal);
        }

        public async Task<bool> UpdateOugGoalAsync(GoalAPIin goalIn)
        {
            var item = await this._db.GetOugGoalByIdAsync(goalIn.Id);
            if(item == null)
            {
                return false;
            }
            item.Name = goalIn.Name;
            item.Description = goalIn.Description;
            return await this._db.UpdateOugGoalAsync(item);
        }

        public async Task<bool> DeleteOugGoalAsync(Guid id)
        {
            return await this._db.DeleteOugGoalAsync(id);
        }

        public async Task<bool> FinishGoal(Guid id) {
            OugGoal goal = await _db.GetOugGoalByIdAsync(id);
            return goal.ArchiveGoal();
        }
    }
}
