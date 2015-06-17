#if WPF
using System.Windows;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using Windows.UI.Text;
using CultureArgumentType = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public class BoolToFontWeightConverter : BoolConverterBase<FontWeight>
    {
        protected override FontWeight GetDefaultTrueValue()
        {
            return GetFontWeight(500);
        }

        protected override FontWeight GetDefaultFalseValue()
        {
            return GetFontWeight(1);
        }

        private static FontWeight GetFontWeight(int weight)
        {
#if WPF
            return FontWeight.FromOpenTypeWeight(weight);
#elif WINRT
            return new FontWeight
            {
                Weight = (ushort)weight
            };
#endif
        }
    }
}
