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
            byte a = GetByte(value.Substring(1, 2));
            byte r = GetByte(value.Substring(3, 2));
            byte g = GetByte(value.Substring(5, 2));
            byte b = GetByte(value.Substring(7, 2));
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        protected override string ConvertBack(SolidColorBrush value, Type targetType, object parameter, string language)
        {
            return value != null
                ? value.Color.ToString()
                : null;
        }

        private static byte GetByte(string source)
        {
            return System.Convert.ToByte(source, 16);
        }
    }
}
