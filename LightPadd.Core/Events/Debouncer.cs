using System;
using System.Collections.Concurrent;
using System.Timers;

namespace LightPadd.Core.Events
{
    public static class Debouncer
    {
        private static readonly ConcurrentDictionary<Timer, Action> _debounceInstances = new();

        public static void Debounce(this Timer timer, Action action, TimeSpan interval)
        {
            // If this timer is already running, stop it.
            bool isRunning = timer.Enabled;
            if (isRunning)
            {
                timer.Stop();
            }

            // Reset its parameters
            timer.Elapsed -= Timer_Elapsed;
            timer.Interval = interval.TotalMilliseconds;

            timer.Elapsed += Timer_Elapsed;
            _debounceInstances.AddOrUpdate(timer, action, (k, v) => action);

            timer.Start();
        }

        private static void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (sender is not Timer timer)
            {
                return;
            }

            timer.Elapsed -= Timer_Elapsed;
            timer.Stop();

            if (_debounceInstances.TryRemove(timer, out Action? action))
            {
                action?.Invoke();
            }
        }
    }
}
