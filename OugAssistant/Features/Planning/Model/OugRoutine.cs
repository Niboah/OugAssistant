using System;
using System.ComponentModel.DataAnnotations;
using OugAssistant.Objects;

namespace OugAssistant.Features.Planning.Model;


public class OugRoutine : OugTask
{
    public HashSet<TimeOnly>[] WeekTimes { get; set; } = new HashSet<TimeOnly>[7];
    public OugRoutine() { }
    public OugRoutine(string name, string? description, TaskPriority priority, Guid goalId, OugGoal goal, HashSet<TimeOnly>[] weekTimes) : base(name, description, priority, goalId, goal){
        WeekTimes = weekTimes;
    }

    public void Add(int weekDay, List<TimeOnly> timeDay)
    {
        WeekTimes[weekDay] = timeDay.ToHashSet(); ;
    }

    public DateTime RoutineDateTime()
    {
        return CalcNextsRoutinesDateTime(1).FirstOrDefault();
    }

    private List<DateTime> CalcNextsRoutinesDateTime(int numList = 10)
    {
        List<DateTime> dateTimes = new List<DateTime>();
        DateTime last = DateTime.Now;
        for (int i = 0; i < numList; i++)
        {
            last = CalcNextRoutineDateTime(last);
            dateTimes.Add(last);
        }

        return dateTimes;
    }

    private DateTime CalcNextRoutineDateTime(DateTime now)
    {
        DateTime nextDateTime = now;
        int nowDayOfWeek = now.DayOfWeek == 0 ? 7 : (int)now.DayOfWeek; //Change Sunday to 7

        var weekTimesOrdered = WeekTimes.Select((set, index) => new { Index = index, Set = set })
                 .Where(x => x.Set != null && x.Set.Count > 0)
                 .OrderBy(x => (x.Index - nowDayOfWeek + 7) % 7)
                 .FirstOrDefault();

        TimeOnly timeDay = weekTimesOrdered.Set.FirstOrDefault();
        int weekDay = weekTimesOrdered.Index;

        if (weekDay == null || timeDay == null)
        {
            throw new Exception();
        }

        int daysToAdd = ((int)weekDay - (int)nowDayOfWeek + 7) % 7;
        if (daysToAdd == 0)
            daysToAdd = 7; // Opcional: si quieres saltar al mismo día *la próxima semana*

        nextDateTime = nextDateTime.Date.AddDays(daysToAdd);

        if (timeDay >= TimeOnly.FromTimeSpan(now.TimeOfDay))
        { //Today
            nextDateTime = DateTime.Today + timeDay.ToTimeSpan();
        }
        else
        { // Other day
            nextDateTime = DateTime.Today.AddDays(1) + timeDay.ToTimeSpan();
        }
        return nextDateTime;
    }


}
