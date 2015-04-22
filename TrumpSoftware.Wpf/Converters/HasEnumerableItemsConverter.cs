using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using TrumpSoftware.WPF.Converters;

namespace TrumpSoftware.Wpf.Converters
{
    public class HasEnumerableItemsConverter : ChainConverter<IEnumerable,bool>
    {
        protected override bool Convert(IEnumerable value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value != null && value.OfType<object>().Any());
        }

        protected override IEnumerable ConvertBack(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
