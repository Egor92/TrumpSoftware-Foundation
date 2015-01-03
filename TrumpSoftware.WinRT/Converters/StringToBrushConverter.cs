using System;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace TrumpSoftware.WinRT.Converters
{
    public class StringToBrushConverter : ChainConverter
    {
        protected override object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = (string)value;
            if (str == null)
                return null;
            byte a = GetByte(str.Substring(1, 2));
            byte r = GetByte(str.Substring(3, 2));
            byte g = GetByte(str.Substring(5, 2));
            byte b = GetByte(str.Substring(7, 2));
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var brush = (SolidColorBrush) value;
            if (brush == null)
                return null;
            return brush.Color.ToString();
        }

        private static byte GetByte(string source)
        {
            return System.Convert.ToByte(source, 16);
        }
    }
}
