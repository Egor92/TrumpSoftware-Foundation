using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace TrumpSoftware.WPF.Converters
{
    [ContentProperty("Converter")]
    public abstract class ChainConverter<TFrom, TTo> : MarkupExtension, IValueConverter
    {
        public IValueConverter Converter { get; set; }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Converter != null)
                value = Converter.Convert(value, targetType, parameter, culture);
            if (value != null && !(value is TFrom))
                throw new ArgumentException(string.Format("value is not of type {0}", typeof(TFrom).FullName), "value");
            if (value == null && typeof(TFrom).IsValueType)
                throw new Exception(string.Format("value of type {0} cannot be null", typeof(TFrom).FullName));
            return Convert((TFrom)value, targetType, parameter, culture);
        }

        protected abstract TTo Convert(TFrom value, Type targetType, object parameter, CultureInfo culture);

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Converter != null)
                value = Converter.ConvertBack(value, targetType, parameter, culture);
            if (value != null && !(value is TTo))
                throw new ArgumentException(string.Format("value is not of type {0}", typeof(TTo).FullName), "value");
            if (value == null && typeof(TTo).IsValueType)
                throw new Exception(string.Format("value of type {0} cannot be null", typeof(TTo).FullName));
            return ConvertBack((TTo)value, targetType, parameter, culture);
        }

        protected abstract TFrom ConvertBack(TTo value, Type targetType, object parameter, CultureInfo culture);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
