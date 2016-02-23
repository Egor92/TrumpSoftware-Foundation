using System;
#if WPF
using System.Windows;
using System.Windows.Markup;
using System.Windows.Data;
using TrumpSoftware.Wpf.Converters.Helpers;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using TrumpSoftware.WinRT.Converters.Helpers;
using CultureArgumentType = System.String;

#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
#if WPF
    public abstract class StoppableChainConverter<TFrom, TTo> : MarkupExtension, IChainConverter
#elif WINRT
    public abstract class StoppableChainConverter<TFrom, TTo> : IChainConverter
#endif
    {
        public IValueConverter Converter { get; set; }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            if (value == DependencyProperty.UnsetValue)
                return DependencyProperty.UnsetValue;
            var castedValue = ChainConverterHelper.CastValue<TFrom>(value, this);
            if (CheckToStopChainConverting(castedValue, targetType, parameter, cultureArgument))
                return GetStoppedValue(castedValue, targetType, parameter, cultureArgument);
            var valueToConvert = GetValueToConvert(castedValue, targetType, parameter, cultureArgument);
            if (Converter != null)
                return Converter.Convert(valueToConvert, targetType, parameter, cultureArgument);
            return valueToConvert;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            throw new NotSupportedException();
        }

#if WPF
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
#endif

        protected abstract bool CheckToStopChainConverting(TFrom value, Type targetType, object parameter, CultureArgumentType cultureArgument);

        protected abstract TTo GetStoppedValue(TFrom value, Type targetType, object parameter, CultureArgumentType cultureArgument);

        protected virtual object GetValueToConvert(TFrom value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            return value;
        }
    }
}