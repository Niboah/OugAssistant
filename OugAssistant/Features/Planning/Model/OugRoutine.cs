
using System.Globalization;

namespace OugAssistant.Features.Planning.Model;

public class OugRoutine : OugTask
{
    public int RoutineDays { get; set; }
    public HashSet<TimeOnly>[] Routines { get; set; }  
    public DateTime? LastFinished { get; set; }
    public OugRoutine() { }
    public OugRoutine(string name, string? description, TaskPriority priority, OugTask? parentTask, ICollection<OugGoal> goalList, HashSet<TimeOnly>[] routines) : base(name, description, priority, parentTask, goalList)
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
        LastFinished = this.RoutineDateTime();
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
        int ciclo = Routines.Length;
        if (ciclo == 0) throw new ArgumentException("La matriz debe tener al menos un día.");

        // Total de días transcurridos desde startDate
        int diasTranscurridos = (int)Math.Floor((now.Date - CreatedDateTime.Date).TotalDays);

        // Índice dentro del ciclo
        int diaIndice = diasTranscurridos % ciclo;
        int diaOffset = diasTranscurridos - diaIndice; // días enteros de ciclos completos

        // Buscamos a partir del día actual en el ciclo
        for (int i = 0; i < ciclo; i++)
        {
            int indiceActual = (diaIndice + i) % ciclo;
            DateTime fechaDia = CreatedDateTime.Date.AddDays(diaOffset + i);

            foreach (var hora in Routines[indiceActual])
            {
                DateTime candidato = fechaDia.Add(hora.ToTimeSpan());
                if (candidato > now)
                    return candidato;
            }
        }

        // Si llegamos aquí, el siguiente horario está en el siguiente ciclo completo
        DateTime proximoCiclo = CreatedDateTime.Date.AddDays(diaOffset + ciclo);
        for (int i = 0; i < ciclo; i++)
        {
            foreach (var hora in Routines[i])
            {
                DateTime candidato = proximoCiclo.Add(hora.ToTimeSpan());
                if (candidato > now)
                    return candidato;
            }
        }

        throw new InvalidOperationException("No se pudo encontrar un horario válido. (" + Name + ")");

    }


}
