using System;
using System.Collections.Generic;
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
        public GoalAPIout Goal { get; set; }

        public TaskAPIout(OugTask task) {
            Id = task.Id;
            Name = task.Name;
            Description = task.Description;
            Priority = task.Priority;
            Goal = new GoalAPIout(task.Goal);
        }
    }

    public class EventAPIout : TaskAPIout
    {
        public EventAPIout(OugTask task) : base(task)
        {
            if (task is OugEvent ev)
            {
                EventDateTime = ev.EventDateTime;
                Place = ev.Place;
            }
            else
            {
                EventDateTime = null;
                Place = null;
            }
        }

        public DateTime? EventDateTime { get; set; }
        public string? Place { get; set; }
    }

    public class MissionAPIout : TaskAPIout
    {
        public MissionAPIout(OugTask task) : base(task)
        {
            if (task is OugMission mission)
            {
                DeadLine = mission.DeadLine;
            }
            else
            {
                DeadLine = null;
            }
        }

        public DateTime? DeadLine { get; set; }
    }

    public class RoutineAPIout : TaskAPIout
    {
        public RoutineAPIout(OugTask task) : base(task)
        {
            if (task is OugRoutine routine)
            {
                WeekTimes = routine.WeekTimes;
            }
            else
            {
                WeekTimes = null;
            }
        }

        public HashSet<TimeOnly>[]? WeekTimes { get; set; }
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
