using OugAssistant.Features.Planning.Model;

namespace OugAssistant_APP.Interfaces.IPlanningBD
{
    public interface IPlanningDB
    {
        #region Task
        Task<ICollection<OugTask>> GetOugTaskAsync();
        Task<ICollection<OugTask>> GetOugTaskAsync(ICollection<Guid> idList);
        Task<OugTask?> GetOugTaskByIdAsync(Guid id);
        Task<bool> AddOugTaskAsync(OugTask item);
        Task<bool> AddOugTaskAsync(ICollection<OugTask> itemList);
        Task<bool> UpdateOugTaskAsync(OugTask item);
        Task<bool> UpdateOugTaskAsync(ICollection<OugTask> itemList);
        Task<bool> DeleteOugTaskAsync(Guid id);
        Task<bool> DeleteOugTaskAsync(ICollection<Guid> idList);
        Task<bool> FinishOugTaskAsync(Guid id);
        Task<bool> FinishOugTaskAsync(ICollection<Guid> idList);
        #endregion

        #region Goal
        Task<ICollection<OugGoal>> GetOugGoalAsync();
        Task<ICollection<OugGoal>> GetOugGoalAsync(ICollection<Guid> idList);
        Task<OugGoal?> GetOugGoalByIdAsync(Guid id);
        Task<bool> AddOugGoalAsync(OugGoal item);
        Task<bool> AddOugGoalAsync(ICollection<OugGoal> itemList);
        Task<bool> UpdateOugGoalAsync(OugGoal item);
        Task<bool> UpdateOugGoalAsync(ICollection<OugGoal> itemList);
        Task<bool> DeleteOugGoalAsync(Guid id);
        Task<bool> DeleteOugGoalAsync(ICollection<Guid> idList);
        Task<int> CountGoalChilds(Guid id);
        Task<int> CountGoalChildsTasks(Guid id);
        #endregion
    }
}
