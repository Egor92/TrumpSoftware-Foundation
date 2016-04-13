using System;
using TrumpSoftware.Common.Helpers;

namespace TrumpSoftware.Common.Time
{
    public sealed class Timer : TimerBase
    {
        #region Events

        #region TimeHasExpired

        public event EventHandler TimeHasExpired;

        private void RaiseTimeHasExpired()
        {
            TimeHasExpired?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Ctor

        public Timer()
        {
        }

        public Timer(TimeSpan interval)
            : base(interval)
        {
        }

        #endregion

        #region Overridden members

        protected override TimeSpan AggregateTime(TimeSpan left, TimeSpan right)
        {
            return left - right;
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

        #endregion

        public void Start(TimeSpan newTime)
        {
            Time = newTime;
            Start();
        }
    }
}
