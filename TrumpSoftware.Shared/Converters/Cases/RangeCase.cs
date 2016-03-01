using System;
using TrumpSoftware.Common;

#if WPF
namespace TrumpSoftware.Wpf.Converters.Cases
#elif WINRT
namespace TrumpSoftware.WinRT.Converters.Cases
#endif
{
    public class RangeCase : ICase
    {
        #region Ctor

        public RangeCase()
        {
            Min = Double.MinValue;
            Max = Double.MaxValue;
        }

        #endregion

        public double Min { get; set; }

        public double Max { get; set; }
        public bool IsMinStrictly { get; set; }
        public bool IsMaxStrictly { get; set; }

        public object Value { get; set; }

        public bool IsMatched(object value)
        {
            double @double;
            if (!ConvertEx.TryConvert(value, out @double))
                return false;
            return (IsMinStrictly && Min <= @double || !IsMinStrictly && Min < @double)
                   && (IsMaxStrictly && Max >= @double || !IsMaxStrictly && Max > @double);
        }
    }
}