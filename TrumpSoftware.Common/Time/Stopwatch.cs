using System;

namespace TrumpSoftware.Common
{
    public sealed class Stopwatch : TimerBase
    {
        public Stopwatch()
        {
        }

        public Stopwatch(TimeSpan interval)
            : base(interval)
        {
        }

        protected override TimeSpan GetDelayInterval()
        {
            return Interval;
        }

        protected override Func<TimeSpan, TimeSpan, TimeSpan> GetTimeFunc()
        {
            return (left, right) => left + right;
        }
    }
}
