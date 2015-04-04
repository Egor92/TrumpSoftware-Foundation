using System.Windows;

namespace TrumpSoftware.Wpf.Converters
{
    public class BoolToFontWeightConverter : BoolConverter<FontWeight>
    {
        protected override FontWeight GetDefaultTrueValue()
        {
            return FontWeight.FromOpenTypeWeight(999);
        }

        protected override FontWeight GetDefaultFalseValue()
        {
            return FontWeight.FromOpenTypeWeight(1);
        }
    }
}
