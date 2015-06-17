#if WPF
using System.Globalization;
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
    public class BoolConverter : BoolConverterBase<object>
    {
        protected override object GetDefaultTrueValue()
        {
            return null;
        }

        protected override object GetDefaultFalseValue()
        {
            return null;
        }
    }
}
