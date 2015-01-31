using System;

namespace TrumpSoftware.Common
{
    public sealed class ReverseTimer : TimerBase
    {
        public event EventHandler TimeHasExpired;

        public ReverseTimer()
        {
        }

        public ReverseTimer(TimeSpan interval)
            : base(interval)
        {
        }

        public void Start(TimeSpan newTime)
        {
            Time = newTime;
            Start();
        }

        protected override Func<TimeSpan, TimeSpan, TimeSpan> GetTimeFunc()
        {
            return (left, right) => left - right;
        }

        protected override void OnTimeChanged()
        {
            if (Time.Ticks <= 0)
                RaiseTimeHasExpired();
        }

        protected override TimeSpan GetDelayInterval()
        {
            return TimeSpanHelper.Min(Interval, Time);
        }

        private void RaiseTimeHasExpired()
        {
            var handler = TimeHasExpired;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
