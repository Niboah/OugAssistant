using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public string Description { get; set; }
        public TaskPriority Priority { get; set; }
        public GoalAPIout? Goal { get; set; }
        public string Type { get; set; }
        public DateTime? EventDateTime { get; set; }
        public string? Place { get; set; }
        public DateTime? DeadLine { get; set; }
        public HashSet<TimeOnly>[]? WeekTimes { get; set; }

        public TaskAPIout(OugTask task, bool mapGoal = true, HashSet<Guid>? visited = null) {
            Id = task.Id;
            Name = task.Name;
            Description = task.Description;
            Priority = task.Priority;
            if (mapGoal && task.Goal != null)
                Goal = new GoalAPIout(task.Goal, visited);

            Type = task.GetType().Name;

            if (task is OugEvent ev)
            {
                EventDateTime = ev.EventDateTime;
                Place = ev.Place;
            }
            if (task is OugMission mission)
            {
                DeadLine = mission.DeadLine;
            }
            if (task is OugRoutine routine)
            {
                WeekTimes = routine.WeekTimes;
            }
        }

    }

    public class TaskAPIin
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TaskPriority Priority { get; set; }
        public Guid? GoalId { get; set; }

    }

    public class EventAPIin : TaskAPIin
    {
        public DateTime? EventDateTime { get; set; }
        public string? Place { get; set; }
    }

    public class MissionAPIin : TaskAPIin
    {
        public DateTime? DeadLine { get; set; }
    }

    public class RoutineAPIin : TaskAPIin
    {
        public HashSet<TimeOnly>[]? WeekTimes { get; set; }
    }

}
