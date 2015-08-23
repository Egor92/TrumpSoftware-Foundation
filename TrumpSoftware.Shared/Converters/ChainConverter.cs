using System;
#if WPF
using TrumpSoftware.Wpf.Extensions;
using System.Windows.Data;
using System.Windows.Markup;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using TrumpSoftware.WinRT.Extensions;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using CultureArgumentType = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
{
    [ContentProperty("Converter")]
    public abstract class ChainConverter<TFrom, TTo> : MarkupExtension, IValueConverter
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
{
    [ContentProperty(Name = "Converter")]
    public abstract class ChainConverter<TFrom, TTo> : IValueConverter
#endif
    {
        public IValueConverter Converter { get; set; }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            if (Converter != null)
                value = Converter.Convert(value, targetType, parameter, cultureArgument);
            if (value != null && !(value is TFrom))
                throw new ArgumentException(string.Format("value is not of type {0}", typeof(TFrom).FullName), "value");
            if (value == null && (typeof(TFrom).IsValueType() || !typeof(TFrom).IsNullable()))
                throw new Exception(string.Format("value of type {0} cannot be null", typeof(TFrom).FullName));
            return Convert((TFrom)value, targetType, parameter, cultureArgument);
        }

        protected abstract TTo Convert(TFrom value, Type targetType, object parameter, CultureArgumentType cultureArgument);

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            if (Converter != null)
                value = Converter.ConvertBack(value, targetType, parameter, cultureArgument);
            if (value != null && !(value is TTo))
                throw new ArgumentException(string.Format("value is not of type {0}", typeof(TTo).FullName), "value");
            if (value == null && typeof(TTo).IsValueType())
                throw new Exception(string.Format("value of type {0} cannot be null", typeof(TTo).FullName));
            return ConvertBack((TTo)value, targetType, parameter, cultureArgument);
        }

        protected abstract TFrom ConvertBack(TTo value, Type targetType, object parameter, CultureArgumentType cultureArgument);

#if WPF
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
#endif
    }
}
