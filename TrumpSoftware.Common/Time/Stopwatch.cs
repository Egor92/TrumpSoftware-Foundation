using System;

namespace TrumpSoftware.Common.Time
{
    public sealed class Stopwatch : TimerBase
    {
        #region Ctor

        public Stopwatch()
        {
        }

        public Stopwatch(TimeSpan interval)
            : base(interval)
        {
        }

        #endregion

        #region Overridden members

        protected override TimeSpan GetDelayInterval()
        {
            return Interval;
        }

        protected override TimeSpan AggregateTime(TimeSpan left, TimeSpan right)
        {
            return left + right;
        }

        #endregion
    }
}
