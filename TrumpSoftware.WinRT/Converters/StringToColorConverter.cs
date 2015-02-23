using System;
using Windows.UI;

namespace TrumpSoftware.WinRT.Converters
{
    public class StringToColorConverter : ChainConverter<string,Color>
    {
        protected override Color Convert(string value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return Colors.Transparent;
            return ColorHelper.ConvertFromString(value);
        }

        protected override string ConvertBack(Color value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }
    }
}
