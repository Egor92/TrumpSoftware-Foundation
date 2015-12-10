using System;

#if WPF
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using CultureArgumentType = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public class ObjectToStringConverter : ChainConverter<object, string>
    {
        public string Format { get; set; }

        protected override string Convert(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            if (value == null)
                return null;
            if (Format == null)
                return value.ToString();
            var format = string.Format("{{0:{0}}}", Format);
            return string.Format(format, value);
        }

        protected override object ConvertBack(string value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            throw new NotSupportedException();
        }
    }
}
