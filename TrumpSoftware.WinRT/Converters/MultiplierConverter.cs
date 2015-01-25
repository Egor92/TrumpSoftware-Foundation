using System;

namespace TrumpSoftware.WinRT.Converters
{
    public abstract class MultiplierConverter<T> : ChainConverter<T,T>
    {
        public double Factor { get; set; }

        public MultiplierConverter()
        {
            Factor = 1.0;
        }

        protected override T Convert(T value, Type targetType, object parameter, string language)
        {
            return GetMultipledValue(value, x => x * Factor);
        }

        protected override T ConvertBack(T value, Type targetType, object parameter, string language)
        {
            return GetMultipledValue(value, x => x / Factor);
        }

        protected abstract T GetMultipledValue(T value, Func<double, double> mult);
    }
}
