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

        #region Properties

        public double Min { get; set; }

        public double Max { get; set; }

        public bool IsMinStrictly { get; set; }

        public bool IsMaxStrictly { get; set; }

        public object Value { get; set; }

        #endregion

        public bool IsMatched(object value)
        {
            var convertResult = ConvertEx.TryConvert<double>(value);
            if (!convertResult.IsSuccess)
                return false;

            double @double = convertResult.Data;
            return (IsMinStrictly && Min < @double || !IsMinStrictly && Min <= @double)
                   && (IsMaxStrictly && Max > @double || !IsMaxStrictly && Max >= @double);
        }
    }
}