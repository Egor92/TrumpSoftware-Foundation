using System;
using System.Globalization;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace TrumpSoftware.WinRT.Converters
{
    [ContentProperty(Name = "Converter")]
    public abstract class ChainConverter : IValueConverter
    {
        public IValueConverter Converter { get; set; }

        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (Converter != null)
                value = Converter.Convert(value, targetType, parameter, language);
            return Convert(value, targetType, parameter, language);
        }

        protected abstract object Convert(object value, Type targetType, object parameter, string language);

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (Converter != null)
                value = Converter.ConvertBack(value, targetType, parameter, language);
            return ConvertBack(value, targetType, parameter, language);
        }

        protected abstract object ConvertBack(object value, Type targetType, object parameter, string language);
    }
}
