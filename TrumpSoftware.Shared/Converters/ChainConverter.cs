using System.Windows;
using System;
#if WPF
using System.Windows.Data;
using System.Windows.Markup;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using CultureArgumentType = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
{
    [ContentProperty("Converter")]
    public abstract class ChainConverter<TFrom, TTo> : MarkupExtension, IChainConverter
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
{
    [ContentProperty(Name = "Converter")]
    public abstract class ChainConverter<TFrom, TTo> : IChainConverter
#endif
    {
        public IValueConverter Converter { get; set; }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            if (value == DependencyProperty.UnsetValue)
                return DependencyProperty.UnsetValue;
            var castedValue = this.CastValue<TFrom>(value);
            object convertedValue = Convert(castedValue, targetType, parameter, cultureArgument);
            if (Converter != null)
                convertedValue = Converter.Convert(convertedValue, targetType, parameter, cultureArgument);
            return convertedValue;
        }

        protected abstract TTo Convert(TFrom value, Type targetType, object parameter, CultureArgumentType cultureArgument);

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            if (value == DependencyProperty.UnsetValue)
                return DependencyProperty.UnsetValue;
            var castedValue = this.CastValue<TTo>(value);
            object convertedValue = ConvertBack(castedValue, targetType, parameter, cultureArgument);
            if (Converter != null)
                convertedValue = Converter.ConvertBack(convertedValue, targetType, parameter, cultureArgument);
            return convertedValue;
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
