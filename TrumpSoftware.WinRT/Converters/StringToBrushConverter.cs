using System;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace TrumpSoftware.WinRT.Converters
{
    public class StringToBrushConverter : ChainConverter<string,SolidColorBrush>
    {
        protected override SolidColorBrush Convert(string value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;
            var color = ColorHelper.ConvertFromString(value);
            return new SolidColorBrush(color);
        }

        protected override string ConvertBack(SolidColorBrush value, Type targetType, object parameter, string language)
        {
            return value != null
                ? value.Color.ToString()
                : null;
        }
    }
}
