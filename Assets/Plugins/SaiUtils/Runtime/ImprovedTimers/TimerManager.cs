using System.Collections.Generic;

namespace SaiUtils.ImprovedTimers
{
    public static class TimerManager {
        static readonly List<Timer> timers = new();

        public static void RegisterTimer(Timer timer) => timers.Add(timer);
        public static void DeregisterTimer(Timer timer) => timers.Remove(timer);

        public static void UpdateTimers()
        {
            // Copy the list to avoid concurrent modification
            foreach (var timer in new List<Timer>(timers))
            {
                timer.Tick();
            }
        }

        public static void Clear() => timers.Clear();
    }
}