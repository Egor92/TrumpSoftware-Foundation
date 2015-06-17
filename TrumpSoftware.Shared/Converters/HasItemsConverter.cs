using System;
using System.Collections;
using System.Linq;

#if WPF
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using CultureArgumentType = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public class HasItemsConverter : ChainConverter<IEnumerable,bool>
    {
        protected override bool Convert(IEnumerable value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            return (value != null && value.OfType<object>().Any());
        }

        protected override IEnumerable ConvertBack(bool value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            throw new NotSupportedException();
        }
    }
}
