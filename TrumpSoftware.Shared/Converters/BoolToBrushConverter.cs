#if WPF
using System.Globalization;
using System.Windows.Media;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using Windows.UI;
using Windows.UI.Xaml.Media;
using CultureArgumentType = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public class BoolToBrushConverter : BoolConverterBase<Brush>
    {
        protected override Brush GetDefaultTrueValue()
        {
            return GetDefaultBrush();
        }

        protected override Brush GetDefaultFalseValue()
        {
            return GetDefaultBrush();
        }

        private static Brush GetDefaultBrush()
        {
#if WPF
            return Brushes.Black;
#elif WINRT
            return new SolidColorBrush(Colors.Black);
#endif
        }
    }
}
