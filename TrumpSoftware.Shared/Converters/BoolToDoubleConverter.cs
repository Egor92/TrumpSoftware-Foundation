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
    public class BoolToDoubleConverter : BoolConverterBase<double>
    {
        protected override double GetDefaultFalseValue()
        {
            return 0.0;
        }

        protected override double GetDefaultTrueValue()
        {
            return 1.0;
        }
    }
}
