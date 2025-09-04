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
            try
            {
                IEnumerable<OugTask> tasks = await this._db.GetOugTaskAsync();

                var withDate = tasks.Where(task => task.FinishedDateTime == null).Select(task => new
                {
                    Task = new TaskAPIout(task, true),
                    Date = task switch
                    {
                        OugEvent e => e.EventDateTime,
                        OugMission m => m.DeadLine,
                        OugRoutine r => r.RoutineDateTime(),
                        _ => DateTime.MaxValue // Por si hay un tipo no reconocido
                    }
                });

                var ordered = orderByPriorityFirst
                    ? withDate.OrderBy(t => t.Task.Priority).ThenBy(t => t.Date).Select(t => t.Task)
                    : withDate.OrderBy(t => t.Date).ThenBy(t => t.Task.Priority).Select(t => t.Task);

                return ordered.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("GetAllOugTaskAsync", ex);
            }
        }

        public async Task<TaskAPIout?> GetOugTaskByIdAsync(Guid id, Type? type = null)
        {
            try
            {
                var task = await this._db.GetOugTaskByIdAsync(id);

                return new TaskAPIout(task);
            }
            catch (Exception ex) {
                throw new Exception("GetOugTaskByIdAsync", ex);
            }
        }

        public async Task<bool> AddOugTaskAsync(TaskAPIin item)
        {
            try
            {
                if (item.GoalIdList.Count() == 0)
                    throw new ArgumentException("GoalId no puede ser null");



                ICollection<OugGoal> goalList = await _db.GetOugGoalAsync(item.GoalIdList);
                OugTask? parentTask = item.ParentTaskId!=null ? await _db.GetOugTaskByIdAsync((Guid)item.ParentTaskId) : null;

                OugTask task = item.Type switch
                {
                    "OugEvent" => new OugEvent(
                        name: item.Name,
                        description: item.Description,
                        priority: item.Priority,
                        parentTask: parentTask,
                        goalList: goalList,
                        eventDateTime: (DateTime)item.EventDateTime,
                        place: item.Place
                    ),
                    "OugMission" => new OugMission(
                        name: item.Name,
                        description: item.Description,
                        priority: item.Priority,
                        parentTask: parentTask,
                        goalList: goalList,
                        deadLine: item.DeadLine.Value
                    ),
                    "OugRoutine" => new OugRoutine(
                        name: item.Name,
                        description: item.Description,
                        priority: item.Priority,
                        parentTask: parentTask,
                        goalList: goalList,
                        routines: item.Routines
                    ),
                    _ => throw new InvalidOperationException("Tipo de tarea no soportado o datos incompletos.")
                };

                return await _db.AddOugTaskAsync(task);
            }
            catch (Exception ex) {
                throw new Exception("AddOugTaskAsync", ex);
            }
        }

        public async Task<bool> UpdateOugTaskAsync(TaskAPIin item)
        {
            try
            {
                OugTask task = await this._db.GetOugTaskByIdAsync((Guid)item.Id);
                if (task == null)
                {
                    return false; // Tarea no encontrada
                }
                task.Name = item.Name;
                task.Description = item.Description;
                task.Priority = item.Priority;


                foreach (var newGoalId in item.GoalIdList) { 
                    var existingGoal = task.Goals.FirstOrDefault(g => g.Id == newGoalId);
                    if (existingGoal == null)
                    {
                        OugGoal goal = await _db.GetOugGoalByIdAsync(newGoalId);
                        task.Goals.Add(goal);
                    }
                }

                if (item.Type == "OugEvent")
                {
                    if (item.EventDateTime == null)
                        throw new ArgumentException("EventDateTime no puede ser null para OugEvent");
                    if (task is OugEvent ev)
                    {
                        ev.EventDateTime = item.EventDateTime.Value;
                        ev.Place = item.Place;
                    }
                }
                else if (item.Type == "OugMission")
                {
                    if (item.DeadLine == null)
                        throw new ArgumentException("DeadLine no puede ser null para OugMission");
                    if (task is OugMission mission)
                    {
                        mission.DeadLine = (DateTime)item.DeadLine;
                    }
                }
                else if (item.Type == "OugRoutine")
                {
                    if (item.Routines == null)
                        throw new ArgumentException("WeekTimes no puede ser null para OugRoutine");
                    if (task is OugRoutine routine)
                    {
                        routine.Routines = item.Routines;
                    }
                }
                return await this._db.UpdateOugTaskAsync(task);
            }
            catch (Exception ex) {
                throw new Exception("UpdateOugTaskAsync", ex);
            }
        }

        public async Task<bool> DeleteOugTaskAsync(Guid id)
        {
            try
            {
                return await this._db.DeleteOugTaskAsync(id);
            }
            catch (Exception ex) {
                throw new Exception("DeleteOugTaskAsync", ex);
            }
        }

        public async Task<bool> Finish(Guid id)
        {
            try
            {
                return await _db.FinishOugTaskAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Finish", ex);
            }
        }
    }
}
