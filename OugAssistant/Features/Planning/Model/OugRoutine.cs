using System.ComponentModel.DataAnnotations;

namespace OugAssistant.Features.Planning.Model;

public class OugRoutine
{
    [Key]
    public Guid Id { get; private set; }
    public List<DayOfWeek> WeekDay { get; }
    public List<TimeSpan> TimeDay { get; }
    public OugRoutine() { }
    public OugRoutine(List<DayOfWeek> weekDay, List<TimeSpan> timeDay)
    {
        WeekDay = weekDay;
        TimeDay = timeDay;
    }

    public void Add(DayOfWeek weekDay, TimeSpan timeDay)
    {
        WeekDay.Add(weekDay);
        TimeDay.Add(timeDay);
    }

    public DateTime GetNextRoutineDateTime()
    {
        return CalcNextsRoutinesDateTime(1).FirstOrDefault();
    }

    private List<DateTime> CalcNextsRoutinesDateTime(int numList = 10)
    {
        List<DateTime> dateTimes = new List<DateTime>();
        DateTime last = DateTime.Now;
        for (int i = 0; i < numList; i++)
        {
            dateTimes.Add(CalcNextRoutineDateTime(last));
        }

        return dateTimes;
    }

    private DateTime CalcNextRoutineDateTime(DateTime now)
    {
        DateTime nextDateTime = now;
        DayOfWeek weekDay = WeekDay.OrderBy(x => (x - now.DayOfWeek + 7) % 7).FirstOrDefault();
        TimeSpan timeDay = TimeDay.OrderBy(x =>
        {
            TimeSpan diff = x - now.TimeOfDay;
            return diff < TimeSpan.Zero ? diff + TimeSpan.FromDays(1) : diff;
        }).FirstOrDefault();

        if (weekDay == null || timeDay == null)
        {
            throw new Exception();
        }

        if (weekDay >= now.DayOfWeek)
        { //This week
            nextDateTime = nextDateTime.AddDays(weekDay - now.DayOfWeek);
        }
        else
        { //Next week
            nextDateTime = nextDateTime.AddDays((double)((DayOfWeek.Sunday - now.DayOfWeek) + weekDay));
        }

        if (timeDay >= now.TimeOfDay)
        { //Today
            nextDateTime = nextDateTime.AddMilliseconds((timeDay - now.TimeOfDay).TotalMilliseconds);
        }
        else
        { // Other day
            nextDateTime = nextDateTime.AddMilliseconds((TimeSpan.Zero + timeDay).TotalMilliseconds);
        }
        return nextDateTime;
    }

}

public class OugRoutineTask : OugTask
{

    public DateTime RoutineDateTime { get; set; }

    // FK - Routine
    public Guid RoutineId { get; set; }
    public OugRoutine Routine { get; set; }
    public OugRoutineTask() { }
    public OugRoutineTask(string name, string? description, TaskPriority priority, Guid goalId, OugGoal goal, Guid routineId, OugRoutine routine) : base(name, description, priority, goalId, goal)
    {

        RoutineId = routineId;
        Routine = routine;
        RoutineDateTime = routine.GetNextRoutineDateTime();
    }


}
