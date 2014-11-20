using System.Windows;

namespace TrumpSoftware.Wpf.Converters
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