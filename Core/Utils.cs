using System;
using System.Windows.Media;
using System.Windows.Threading;

namespace Pierre.Core
{
    public static class Utils
    {
        public static Action Throttle(Action action, TimeSpan? timeSpan = null)
        {
            if (!timeSpan.HasValue || timeSpan.Value <= TimeSpan.Zero)
            {
                return () => action.Invoke();
            }
            DispatcherTimer timer = null;
            DateTime lastTime = DateTime.MinValue;
            return () =>
            {
                timer?.Stop();
                var now = DateTime.Now;
                var diff = now - lastTime - timeSpan.Value;
                if (diff >= TimeSpan.Zero)
                {
                    lastTime = now;
                    action.Invoke();
                    return;
                }
                timer = new DispatcherTimer { Interval = -diff };
                timer.Tick += (_, __) =>
                {
                    lastTime = now;
                    action.Invoke();
                    timer?.Stop();
                };
                timer.Start();
            };
        }

        public static Action Debounce(Action action, TimeSpan? timeSpan = null)
        {
            if (!timeSpan.HasValue || timeSpan.Value <= TimeSpan.Zero)
            {
                return () => action.Invoke();
            }
            DispatcherTimer timer = null;
            return () =>
            {
                timer?.Stop();
                timer = new DispatcherTimer { Interval = timeSpan.Value };
                timer.Tick += (_, __) =>
                {
                    action.Invoke();
                    timer?.Stop();
                };
                timer.Start();
            };
        }

        public static Color ColorWithOpacity(Color color, double opacity)
        {
            color.A = (byte)(color.A * opacity);
            return color;
        }

        public static int ColorToBgr(Color color)
        {
            return (color.R << 0)
                | (color.G << 8)
                | (color.B << 16)
                | (color.A << 24);
        }
    }
}
