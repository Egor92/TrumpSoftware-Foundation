using System;
using System.Globalization;
using System.Windows.Media;
using TrumpSoftware.WPF.Converters;

namespace TrumpSoftware.Wpf.Converters
{
    public class StringToBrushConverter : ChainConverter<string,SolidColorBrush>
    {
        protected override SolidColorBrush Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            var color = ColorHelper.ConvertFromString(value);
            return new SolidColorBrush(color);
        }

        protected override string ConvertBack(SolidColorBrush value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null
                ? value.Color.ToString()
                : null;
        }
    }
}
