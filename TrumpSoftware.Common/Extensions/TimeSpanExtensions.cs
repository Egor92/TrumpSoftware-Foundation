using System;

namespace TrumpSoftware.Common.Extensions
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Mult(this TimeSpan source, double multiplier)
        {
            var targetTicks = (long)(source.Ticks * multiplier);
            return TimeSpan.FromTicks(targetTicks);
        }
    }
}
