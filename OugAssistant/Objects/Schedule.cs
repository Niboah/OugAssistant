
namespace OugAssistant.Objects
{
    public class Schedule<T>
    {
        private Dictionary<DateOnly, Dictionary<TimeOnly, T>> _schedule;

        public Schedule()
        {
            _schedule = new Dictionary<DateOnly, Dictionary<TimeOnly, T>>();
        }

        public void AddEntry(DateOnly date, TimeOnly time, T value)
        {
            if (!_schedule.ContainsKey(date))
            {
                _schedule[date] = new Dictionary<TimeOnly, T>();
            }

            _schedule[date][time] = value;
        }

        public bool TryGetEntry(DateOnly date, TimeOnly time, out T value)
        {
            value = default!;
            if (_schedule.TryGetValue(date, out var timeDict))
            {
                return timeDict.TryGetValue(time, out value);
            }
            return false;
        }

        public bool RemoveEntry(DateOnly date, TimeOnly time)
        {
            if (_schedule.TryGetValue(date, out var timeDict))
            {
                bool removed = timeDict.Remove(time);
                if (timeDict.Count == 0)
                {
                    _schedule.Remove(date);
                }
                return removed;
            }
            return false;
        }

        public Dictionary<DateOnly, Dictionary<TimeOnly, T>> GetAllEntries()
        {
            return _schedule;
        }
    }

    public class Schedule : Schedule<object?> { }
}
