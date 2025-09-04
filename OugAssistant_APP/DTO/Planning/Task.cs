using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OugAssistant.Features.Planning.Model;

namespace OugAssistant_APP.DTO.Planning
{
    public class TaskAPIout
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Priority { get; set; }
        public ICollection<GoalAPIout> GoalList { get; set; } = new List<GoalAPIout>();
        public TaskAPIout? ParentTask { get; set; }
        public string Type { get; set; }
        public DateTime? EventDateTime { get; set; }
        public string? Place { get; set; }
        public DateTime? DeadLine { get; set; }
        public DateTime RoutineDatetime { get; set; }
        public HashSet<TimeOnly>[] Routines { get; set; }

        public TaskAPIout(OugTask task, bool mapGoal = true, Dictionary<Guid, Object>? visited = null)
        {

            visited ??= new Dictionary<Guid, Object>();
            bool first = visited.Count() == 0;

            if (visited.ContainsKey(task.Id))
            {
                var existing = (TaskAPIout)visited[task.Id];
                Id = existing.Id;
                Name = existing.Name;
                Description = existing.Description;
                Priority = existing.Priority;
                ParentTask = existing.ParentTask;
                GoalList = existing.GoalList;
                Type = existing.Type;
                EventDateTime = existing.EventDateTime;
                Place = existing.Place;
                DeadLine = existing.DeadLine;
                Routines = existing.Routines;
                return;
            }

            // Agregar antes de las recursiones
            visited.Add(task.Id, this);

            Id = task.Id;
            Name = task.Name;
            Description = task.Description;


            if (mapGoal)
            {
                foreach (var goal in task.Goals)
                {
                    if (visited.ContainsKey(goal.Id))
                    {
                        if (first) GoalList.Add((GoalAPIout)visited[goal.Id]);
                    }
                    else
                    {
                        var goalOut = new GoalAPIout(goal, visited);
                        GoalList.Add(goalOut);
                    }
                }
            }

            ParentTask = task.Parent != null
                ? (visited.ContainsKey(task.Parent.Id)
                    ? (first ? (TaskAPIout)visited[task.Parent.Id] : null)
                    : new TaskAPIout(task.Parent, mapGoal, visited))
                : null;


            Type = task.GetType().Name;

            if (task is OugEvent ev)
            {
                EventDateTime = ev.EventDateTime;
                Place = ev.Place;
            }
            else if (task is OugMission mission)
            {
                DeadLine = mission.DeadLine;
            }
            else if (task is OugRoutine routine)
            {
                Routines = routine.Routines;
                RoutineDatetime = routine.RoutineDateTime();
            }

            Priority = (int)(((ParentTask != null ? (int)ParentTask.Priority : 1) * (int)task.Priority) + GoalList.Sum(g => g.Priority)); 
        }

    }

    public class TaskAPIin
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "El campo Name es obligatorio")]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "El campo Priority es obligatorio")]
        public TaskPriority Priority { get; set; }

        [Required(ErrorMessage = "El campo Type es obligatorio")]
        public string Type { get; set; }

        [MinLength(1, ErrorMessage = "Debe asignar al menos un GoalId")]
        public ICollection<Guid> GoalIdList { get; set; }
        public Guid? ParentTaskId { get; set; }
        public DateTime? EventDateTime { get; set; }
        public string? Place { get; set; }
        public DateTime? DeadLine { get; set; }
        public HashSet<TimeOnly>[]? Routines { get; set; }
    }
}
