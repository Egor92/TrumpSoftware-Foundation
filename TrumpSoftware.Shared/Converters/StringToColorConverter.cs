using System;

#if WPF
using System.Windows.Media;
using TrumpSoftware.Wpf.Media;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using Windows.UI;
using CultureArgumentType = System.String;
using ColorHelper = TrumpSoftware.WinRT.Media.ColorHelper;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public class StringToColorConverter : ChainConverter<string,Color>
    {
        protected override Color Convert(string value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            if (value == null)
                return Colors.Transparent;
            return ColorHelper.ConvertFromString(value);
        }

        protected override string ConvertBack(Color value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            return value.ToString();
        }
    }
}
