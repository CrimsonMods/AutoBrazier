using ProjectM;

namespace AutoBrazier.Server
{
    internal class DayNightCycleTracker
    {
        public delegate void Event(TimeOfDay timeOfDay);

        public event Event OnTimeOfDayChanged;

        private TimeOfDay _previousTimeOfDay;

        public void Update(DayNightCycle dayNightCycle)
        {
            if (dayNightCycle.TimeOfDay == _previousTimeOfDay) return;
            _previousTimeOfDay = dayNightCycle.TimeOfDay;
            OnTimeOfDayChanged?.Invoke(dayNightCycle.TimeOfDay);
        }
    }
}
