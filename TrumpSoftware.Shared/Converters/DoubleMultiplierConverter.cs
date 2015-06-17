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
    public class DoubleMultiplierConverter : MultiplierConverter<double>
    {
        protected override double GetMultipledValue(double value, Func<double, double> mult)
        {
            return mult(value);
        }
    }
}
