using System;

#if WPF
using System.Windows.Media;
using TrumpSoftware.Wpf.Helpers;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using TrumpSoftware.WinRT.Helpers;
using Windows.UI.Xaml.Media;
using CultureArgumentType = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public class StringToBrushConverter : ChainConverter<string,SolidColorBrush>
    {
        protected override SolidColorBrush Convert(string value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            if (value == null)
                return null;
            var color = ColorHelper.ConvertFromString(value);
            return new SolidColorBrush(color);
        }

        protected override string ConvertBack(SolidColorBrush value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            return value != null
                ? value.Color.ToString()
                : null;
        }
    }
}
