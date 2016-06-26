using System;
using System.Collections.Generic;
#if WPF
using System.Windows.Markup;
using TrumpSoftware.Wpf.Converters.Cases;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using TrumpSoftware.WinRT.Converters.Cases;
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
    [ContentProperty("Cases")]
#elif WINRT
    [ContentProperty(Name = "Cases")]
#endif
    public class SwitchConverter : ChainConverter<object, object>, IHaveCasesChainConverter
    {
        public SwitchConverter()
        {
            Cases = new List<ICase>();
        }

        public List<ICase> Cases { get; set; }
        public object Default { get; set; }

        protected override object Convert(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            foreach (var @case in Cases)
            {
                if (@case.IsMatched(value))
                    return @case.Value;
            }
            return Default;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            throw new NotSupportedException();
        }
    }
}