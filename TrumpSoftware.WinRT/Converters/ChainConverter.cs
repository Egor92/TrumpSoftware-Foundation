using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace TrumpSoftware.WinRT.Converters
{
    [ContentProperty(Name = "Converter")]
    public abstract class ChainConverter<TFrom, TTo> : IValueConverter
    {
        public IValueConverter Converter { get; set; }

        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (Converter != null)
                value = Converter.Convert(value, targetType, parameter, language);
            if (!(value is TFrom))
                throw new ArgumentException(string.Format("value is not of type {0}", typeof(TFrom).FullName), "value");
            return Convert((TFrom)value, targetType, parameter, language);
        }

        protected abstract TTo Convert(TFrom value, Type targetType, object parameter, string language);

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (Converter != null)
                value = Converter.ConvertBack(value, targetType, parameter, language);
            if (!(value is TTo))
                throw new ArgumentException(string.Format("value is not of type {0}", typeof(TTo).FullName), "value");
            return ConvertBack((TTo)value, targetType, parameter, language);
        }

        protected abstract TFrom ConvertBack(TTo value, Type targetType, object parameter, string language);
    }
}
