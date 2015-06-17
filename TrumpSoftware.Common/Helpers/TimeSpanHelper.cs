using System;

namespace TrumpSoftware.Common.Helpers
{
    public static class TimeSpanHelper
    {
        public static TimeSpan Min(TimeSpan left, TimeSpan right)
        {
            var ticks = Math.Min(left.Ticks, right.Ticks);
            return new TimeSpan(ticks);
        }

        public static TimeSpan Max(TimeSpan left, TimeSpan right)
        {
            var ticks = Math.Max(left.Ticks, right.Ticks);
            return new TimeSpan(ticks);
        }
    }
}
