using System;
using System.Globalization;
using TrumpSoftware.WPF.Converters;

namespace TrumpSoftware.Wpf.Converters
{
    public class BoolToNegativeConverter : ChainConverter<bool,bool>
    {
        protected override bool Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return !value;
        }

        protected override bool ConvertBack(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return !value;
        }
    }
}
