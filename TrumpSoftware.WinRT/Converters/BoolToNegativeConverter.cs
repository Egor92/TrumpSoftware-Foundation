using System;

namespace TrumpSoftware.WinRT.Converters
{
    public class BoolToNegativeConverter : ChainConverter<bool,bool>
    {
        protected override bool Convert(bool value, Type targetType, object parameter, string language)
        {
            return !value;
        }

        protected override bool ConvertBack(bool value, Type targetType, object parameter, string language)
        {
            return !value;
        }
    }
}
