using System;
#if WPF
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using TrumpSoftware.Wpf.Converters.Helpers;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using TrumpSoftware.WinRT.Converters.Helpers;
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
            var castedValue = ChainConverterHelper.CastValue<TFrom>(value, this);
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
            var castedValue = ChainConverterHelper.CastValue<TTo>(value, this);
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
