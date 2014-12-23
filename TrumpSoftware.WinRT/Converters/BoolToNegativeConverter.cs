using System;

namespace TrumpSoftware.WinRT.Converters
{
    public class BoolToNegativeConverter : ChainConverter
    {
        protected override object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool))
                return null;
            var b = (bool)value;
            return !b;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool))
                return null;
            var b = (bool)value;
            return !b;
        }
    }
}
