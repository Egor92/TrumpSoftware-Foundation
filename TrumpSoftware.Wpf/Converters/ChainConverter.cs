using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace TrumpSoftware.WPF.Converters
{
    [ContentProperty("Converter")]
    public abstract class ChainConverter : MarkupExtension, IValueConverter
    {
        public IValueConverter Converter { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Converter != null)
                value = Converter.Convert(value, targetType, parameter, culture);
            return Convert(value, targetType, parameter, culture);
        }

        protected abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Converter != null)
                value = Converter.ConvertBack(value, targetType, parameter, culture);
            return ConvertBack(value, targetType, parameter, culture);
        }

        protected abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}
