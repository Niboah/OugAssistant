using OugAssistant_APP.DTO.Planning;

namespace OugAssistant_APP.Interfaces.Planning
{
    public interface ITaskServices
    {
        Task<IEnumerable<TaskAPIout>> GetAllOugTaskAsync(bool orderByPriorityFirst = false, Type? type = null);
        Task<TaskAPIout?> GetOugTaskByIdAsync(Guid id, Type? type = null);
        Task<bool> AddOugTaskAsync(TaskAPIin item);
        Task<bool> UpdateOugTaskAsync(TaskAPIin item);
        Task<bool> DeleteOugTaskAsync(Guid id);
        Task<bool> Finish(Guid id);
    }
}
