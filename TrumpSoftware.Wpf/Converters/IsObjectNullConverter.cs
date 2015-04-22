using System;
using System.Globalization;
using TrumpSoftware.WPF.Converters;

namespace TrumpSoftware.Wpf.Converters
{
    public class IsObjectNullConverter : ChainConverter<object,bool>
    {
        protected override bool Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        protected override object ConvertBack(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
