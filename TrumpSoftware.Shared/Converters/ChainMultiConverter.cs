using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System;
#if WPF
using System.Windows;
using System.Windows.Markup;
using TrumpSoftware.Wpf.Converters.Helpers;
#elif WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using TrumpSoftware.WinRT.Converters.Helpers;
using IValueConverter = Windows.UI.Xaml.Data.IValueConverter;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
{
    [ContentProperty("Converter")]
    public abstract class ChainMultiConverter<TFrom, TTo> : MarkupExtension, IChainMultiConverter
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
{
    [ContentProperty(Name = "Converter")]
    public abstract class ChainMultiConverter<TFrom, TTo> : IChainMultiConverter
#endif
    {
        public IValueConverter Converter { get; set; }

        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(x => Equals(x, DependencyProperty.UnsetValue)))
                return DependencyProperty.UnsetValue;
            var castedValues = values.Select(x => ChainConverterHelper.CastValue<TFrom>(x, this)).ToArray();
            object convertedValue = Convert(castedValues, targetType, parameter, culture);
            if (Converter != null)
            {
                var innerCultureArgument = GetCultureArgumentForInnerConverter(culture);
                convertedValue = Converter.Convert(convertedValue, targetType, parameter, innerCultureArgument);
            }
            return convertedValue;
        }

        protected abstract TTo Convert(TFrom[] value, Type targetType, object parameter, CultureInfo culture);

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue)
                return Enumerable.Repeat(DependencyProperty.UnsetValue, targetTypes.Length).ToArray();
            var castedValue = ChainConverterHelper.CastValue<TTo>(value, this);
            return ConvertBack(castedValue, targetTypes, parameter, culture).Cast<object>().ToArray();
        }

        protected abstract TFrom[] ConvertBack(TTo value, Type[] targetTypes, object parameter, CultureInfo culture);

#if WPF
        private static CultureInfo GetCultureArgumentForInnerConverter(CultureInfo culture)
        {
            return culture;
        }
#elif WINRT
        private static string GetCultureArgumentForInnerConverter(CultureInfo culture)
        {
            return culture.Name;
        }
#endif
    }
}