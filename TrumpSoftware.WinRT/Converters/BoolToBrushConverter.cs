using Windows.UI;
using Windows.UI.Xaml.Media;

namespace TrumpSoftware.WinRT.Converters
{
    public class BoolToBrushConverter : BoolConverter<Brush>
    {
        protected override Brush GetDefaultTrueValue()
        {
            return new SolidColorBrush(Colors.Black);
        }

        protected override Brush GetDefaultFalseValue()
        {
            return new SolidColorBrush(Colors.Black);
        }
    }
}
