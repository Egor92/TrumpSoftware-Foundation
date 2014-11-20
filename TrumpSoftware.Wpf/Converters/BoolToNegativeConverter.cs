using System;
using System.Globalization;
using TrumpSoftware.WPF.Converters;

namespace TrumpSoftware.Wpf.Converters
{
    public class BoolToNegativeConverter : ChainConverter
    {
        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return null;
            var b = (bool)value;
            return !b;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return null;
            var b = (bool)value;
            return !b;
        }
    }
}
