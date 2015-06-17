using System;

#if WPF
using System.Windows;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using Windows.UI.Xaml;
using CultureArgumentType = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public class DoubleToGridLenghtConverter : ChainConverter<double,GridLength>
    {
        protected override GridLength Convert(double value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            return new GridLength(value, GridUnitType.Pixel);
        }

        protected override double ConvertBack(GridLength value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            return value.Value;
        }
    }
}
