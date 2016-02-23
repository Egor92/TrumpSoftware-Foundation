using System;
#if WPF
using System.Windows.Data;
using System.Windows.Markup;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using CultureArgumentType = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
#if WPF
    [ContentProperty("Converter")]
    public class ConverterDecorator : MarkupExtension, IChainConverter
#elif WINRT
    [ContentProperty(Name = "Converter")]
    public class ConverterDecorator : IChainConverter
#endif
    {
        public IValueConverter Converter { get; set; }

        public IValueConverter DecoratingConverter { get; set; }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            if (DecoratingConverter != null)
                value = DecoratingConverter.Convert(value, targetType, parameter, cultureArgument);
            if (Converter != null)
                value = Converter.Convert(value, targetType, parameter, cultureArgument);
            return value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            if (DecoratingConverter != null)
                value = DecoratingConverter.ConvertBack(value, targetType, parameter, cultureArgument);
            if (Converter != null)
                value = Converter.ConvertBack(value, targetType, parameter, cultureArgument);
            return value;
        }

#if WPF
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
#endif
    }
}
