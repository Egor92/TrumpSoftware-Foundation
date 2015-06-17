using System;

#if WPF
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using CultureArgumentType = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public abstract class MultiplierConverter<T> : ChainConverter<T,T>
    {
        public double Factor { get; set; }

        public MultiplierConverter()
        {
            Factor = 1.0;
        }

        protected override T Convert(T value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            return GetMultipledValue(value, x => x * Factor);
        }

        protected override T ConvertBack(T value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            return GetMultipledValue(value, x => x / Factor);
        }

        protected abstract T GetMultipledValue(T value, Func<double, double> mult);
    }
}
