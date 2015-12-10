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
    public class InvertBoolConverter : ChainConverter<bool,bool>
    {
        protected override bool Convert(bool value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            return !value;
        }

        protected override bool ConvertBack(bool value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            return !value;
        }
    }
}
