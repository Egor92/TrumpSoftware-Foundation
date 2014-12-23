using Windows.UI.Xaml;

namespace TrumpSoftware.WinRT.Converters
{
    public class BoolToVisibilityConverter : BoolConverter<Visibility>
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