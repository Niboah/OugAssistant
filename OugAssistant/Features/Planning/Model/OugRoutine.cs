
namespace OugAssistant.Features.Planning.Model;

public class OugRoutine : OugTask
{
    public int RoutineDays { get; set; }
    public HashSet<TimeOnly>[] Routines { get; set; }
    public DateTime? LastFinished { get; set; }
    public OugRoutine() { }
    public OugRoutine(string name, string? description, TaskPriority priority, OugTask? parentTask, ICollection<OugGoal> goalList, HashSet<TimeOnly>[] routines): base(name, description, priority, parentTask, goalList)
    {
        Routines = routines;
        RoutineDays = routines.Length;
    }

    public void Add(int weekDay, List<TimeOnly> timeDay)
    {
        if (weekDay >= 0 && weekDay < RoutineDays)
            Routines[weekDay] = timeDay.ToHashSet(); ;
    }

    public DateTime RoutineDateTime()
    {
        return CalcNextsRoutinesDateTime(1).FirstOrDefault();
    }

    public override bool Finish()
    {
        LastFinished = DateTime.Now;
        return true;
    }

    private List<DateTime> CalcNextsRoutinesDateTime(int numList = 10)
    {
        List<DateTime> dateTimes = new List<DateTime>();
        DateTime last = (DateTime)(LastFinished != default ? LastFinished : DateTime.Now);
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

        var weekTimesOrdered = Routines.Select((set, index) => new { Index = index+1, Set = set })
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
