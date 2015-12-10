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
    public class IsNullConverter : ChainConverter<object,bool>
    {
        protected override bool Convert(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            return value == null;
        }

        protected override object ConvertBack(bool value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            throw new NotSupportedException();
        }
    }
}
