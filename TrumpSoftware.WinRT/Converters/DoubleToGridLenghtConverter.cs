using System;
using Windows.UI.Xaml;

namespace TrumpSoftware.WinRT.Converters
{
    public class DoubleToGridLenghtConverter : ChainConverter<double,GridLength>
    {
        protected override GridLength Convert(double value, Type targetType, object parameter, string language)
        {
            return new GridLength(value, GridUnitType.Pixel);
        }

        protected override double ConvertBack(GridLength value, Type targetType, object parameter, string language)
        {
            return value.Value;
        }
    }
}
