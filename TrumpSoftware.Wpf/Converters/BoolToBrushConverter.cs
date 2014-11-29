using System.Windows.Media;
using TrumpSoftware.Wpf.Converters;

namespace TrumpSoftware.WPF.Converters
{
    public class BoolToBrushConverter : BoolConverter<Brush>
    {
        protected override Brush GetDefaultTrueValue()
        {
            return Brushes.Black;
        }

        protected override Brush GetDefaultFalseValue()
        {
            return Brushes.Black;
        }
    }
}
