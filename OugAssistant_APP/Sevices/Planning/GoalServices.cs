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
            try
            {
                var goals = await this._db.GetOugGoalAsync();
                var goalWithChildCounts = new List<(OugGoal goal, int childCount)>();

                foreach (var g in goals)
                {
                    //int count = await this._db.CountGoalChilds(g.Id); 
                    int countTask =  await this._db.CountGoalChildsTasks(g.Id);
                    goalWithChildCounts.Add((g, countTask));
                }

                var orderedGoals = goalWithChildCounts
                    .OrderBy(x => x.goal.Tasks.Count + x.childCount  == 0 ? 0 : 1)
                    .ThenByDescending(x => x.goal.Level)
                    .ThenBy(x => x.childCount)
                    .Select(x => new GoalAPIout(x.goal))
                    .ToList();

                return orderedGoals;
            }
            catch (Exception ex)
            {
                throw new Exception("GetAllOugGoalAsync", ex);
            }
        }

        public async Task<GoalAPIout?> GetOugGoalByIdAsync(Guid id)
        {
            try
            {
                var goal = await this._db.GetOugGoalByIdAsync(id);
                return new GoalAPIout(goal);
            }
            catch (Exception ex)
            {
                throw new Exception("GetOugGoalByIdAsync", ex);
            }
        }

        public async Task<bool> AddOugGoalAsync(GoalAPIin goalIn)
        {
            try
            {
                OugGoal? parentGoal = null;
                if (goalIn.ParentGoalId != null)
                {
                    parentGoal = await this._db.GetOugGoalByIdAsync((Guid)goalIn.ParentGoalId);
                }
                OugGoal goal = new OugGoal(goalIn.Name, goalIn.Description, parentGoal);
                return await this._db.AddOugGoalAsync(goal);
            }
            catch (Exception ex)
            {
                throw new Exception("AddOugGoalAsync", ex);
            }
        }

        public async Task<bool> UpdateOugGoalAsync(GoalAPIin goalIn)
        {
            try
            {
                var item = await this._db.GetOugGoalByIdAsync((Guid)goalIn.Id);
                if (item == null)
                {
                    return false;
                }
                item.Name = goalIn.Name;
                item.Description = goalIn.Description;
                return await this._db.UpdateOugGoalAsync(item);
            }
            catch (Exception ex)
            {
                throw new Exception("UpdateOugGoalAsync", ex);
            }
        }

        public async Task<bool> DeleteOugGoalAsync(Guid id)
        {
            try
            {
                return await this._db.DeleteOugGoalAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("DeleteOugGoalAsync", ex);
            }
        }

        public async Task<bool> FinishGoal(Guid id)
        {
            try
            {
                OugGoal goal = await _db.GetOugGoalByIdAsync(id);
                return goal.ArchiveGoal();
            }
            catch (Exception ex)
            {
                throw new Exception("FinishGoal", ex);
            }
        }
    }
}
