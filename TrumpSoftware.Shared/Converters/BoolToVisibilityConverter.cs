#if WPF
using System.Globalization;
using System.Windows;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using Windows.UI.Xaml;
using CultureArgumentType = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public class BoolToVisibilityConverter : BoolConverterBase<Visibility>
    {
        protected override Visibility GetDefaultTrueValue()
        {
            return Visibility.Visible;
        }

        protected override Visibility GetDefaultFalseValue()
        {
            return Visibility.Collapsed;
        }
    }
}