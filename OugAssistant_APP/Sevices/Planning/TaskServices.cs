using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OugAssistant.Features.Planning.Model;
using OugAssistant_APP.DTO.Planning;
using OugAssistant_APP.Interfaces.IPlanningBD;
using OugAssistant_APP.Interfaces.Planning;

namespace OugAssistant_APP.Sevices.Planning
{
    public class TaskServices : ITaskServices
    {
        private readonly IPlanningDB _db;

        public TaskServices(IPlanningDB db)
        {
            this._db = db;
        }

        public async Task<IEnumerable<TaskAPIout>> GetAllOugTaskAsync(bool orderByPriorityFirst, Type? type = null)
        {
            var tasks = await this._db.GetAllOugTaskAsync(type);

            var withDate = tasks.Where(task=>task.FinishDateTime==null).Select(task => new
            {
                Task = task,
                Date = task switch
                {
                    OugEvent e => e.EventDateTime,
                    OugMission m => m.DeadLine,
                    OugRoutine r => r.RoutineDateTime(),
                    _ => DateTime.MaxValue // Por si hay un tipo no reconocido
                }
            });

            var ordered = orderByPriorityFirst
                ? withDate.OrderBy(t => t.Task.Priority).ThenBy(t => t.Date).Select(t => new TaskAPIout(t.Task))
                : withDate.OrderBy(t => t.Date).ThenBy(t => t.Task.Priority).Select(t => new TaskAPIout(t.Task));

            return ordered.ToList();
        }

        public async Task<TaskAPIout?> GetOugTaskByIdAsync(Guid id, Type? type = null)
        {
            var task = await this._db.GetOugTaskByIdAsync(id, type);

            return new TaskAPIout(task);
        }

        public async Task<bool> AddOugTaskAsync(TaskAPIin item)
        {
            if (item.GoalId == null)
                throw new ArgumentException("GoalId no puede ser null");

            OugGoal goal = await _db.GetOugGoalByIdAsync(item.GoalId.Value);

            OugTask task = item switch
            {
                EventAPIin e when e.EventDateTime != null => new OugEvent(
                    name: e.Name,
                    description: e.Description,
                    priority: e.Priority,
                    goalId: e.GoalId.Value,
                    goal: goal,
                    eventDateTime: e.EventDateTime.Value,
                    place: e.Place
                ),
                MissionAPIin m when m.DeadLine != null => new OugMission(
                    name: m.Name,
                    description: m.Description,
                    priority: m.Priority,
                    goalId: m.GoalId.Value,
                    goal: goal,
                    deadLine: m.DeadLine.Value
                ),
                RoutineAPIin r when r.WeekTimes != null => new OugRoutine(
                    name: r.Name,
                    description: r.Description,
                    priority: r.Priority,
                    goalId: r.GoalId.Value,
                    goal: goal,
                    weekTimes: r.WeekTimes
                ),
                _ => throw new InvalidOperationException("Tipo de tarea no soportado o datos incompletos.")
            };

            return await _db.AddOugTaskAsync(task);
        }

        public async Task<bool> UpdateOugTaskAsync(TaskAPIin item)
        {
            OugTask task = await this._db.GetOugTaskByIdAsync(item.Id);
            if (task == null)
            {
                return false; // Tarea no encontrada
            }
            task.Name = item.Name;
            task.Description = item.Description;
            task.Priority = item.Priority;
            
            if (task.GoalId != item.GoalId) {
                OugGoal goal = await _db.GetOugGoalByIdAsync((Guid)item.GoalId);
                task.GoalId = (Guid)item.GoalId;
                task.Goal = goal;
            }

            if (item is EventAPIin e)
            {
                if (e.EventDateTime == null)
                    throw new ArgumentException("EventDateTime no puede ser null para OugEvent");
                if (task is OugEvent ev)
                {
                    ev.EventDateTime = e.EventDateTime.Value;
                    ev.Place = e.Place;
                }
            }
            else if (item is MissionAPIin m)
            {
                if (m.DeadLine == null)
                    throw new ArgumentException("DeadLine no puede ser null para OugMission");
                if (task is OugMission mission)
                {
                    mission.DeadLine = m.DeadLine.Value;
                }
            }
            else if (item is RoutineAPIin r)
            {
                if (r.WeekTimes == null)
                    throw new ArgumentException("WeekTimes no puede ser null para OugRoutine");
                if (task is OugRoutine routine)
                {
                    routine.WeekTimes = r.WeekTimes;
                }
            }
            return await this._db.UpdateOugTaskAsync(task);
        }

        public async Task<bool> DeleteOugTaskAsync(Guid id)
        {
            return await this._db.DeleteOugTaskAsync(id);
        }

        public async Task<bool> Finish(Guid id)
        {
            OugTask task = await _db.GetOugTaskByIdAsync(id);
            if (task == null)
            {
                return false; // Tarea no encontrada
            }
            return task.Finish();
        }
    }
}
