
namespace OugAssistant.Features.Planning;

internal class Routine(DayOfWeek weekDay, TimeOnly timeDay) : Task
{
    DayOfWeek WeekDay { get; set; } = weekDay;
    TimeOnly TimeDay { get; set; } = timeDay;
}

internal class RoutineTask(Routine routine, DateTime dateTime)
{
    Routine Routine { get; set; } = routine;
    DateTime DateTime { get; set; } = dateTime;
}
