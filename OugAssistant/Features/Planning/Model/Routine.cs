using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OugAssistant.Features.Planning.Model;

public class Routine : Task
{
    public DayOfWeek WeekDay { get; set; }
    public TimeOnly TimeDay { get; set; } 

    public Routine() { }
    public Routine(DayOfWeek weekDay, TimeOnly timeDay) {
        WeekDay = weekDay;
        TimeDay = timeDay;
    }
}
[Keyless]
public class RoutineTask
{
    public Routine Routine { get; set; }
    public DateTime DateTime { get; set; }

    public RoutineTask() {
    }

    public RoutineTask(Routine routine, DateTime dateTime) {
        Routine = routine;
        DateTime = dateTime;
    }
}
