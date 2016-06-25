using System;
using System.Collections.Generic;
using System.Linq;
#if WPF
using System.Windows.Markup;
using TrumpSoftware.Wpf.Converters.Cases;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using Windows.UI.Xaml.Markup;
using TrumpSoftware.WinRT.Converters.Cases;
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
    public class StopIfConverter : StoppableChainConverter<object, object>, IHaveCasesChainConverter
    {
        public StopIfConverter()
        {
            Cases = new List<ICase>();
        }

        public List<ICase> Cases { get; set; }

        protected override bool CheckToStopChainConverting(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            return Cases.Any(x => x.IsMatched(value));
        }

        protected override object GetStoppedValue(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            var @case = Cases.FirstOrDefault(x => x.IsMatched(value));
            if (@case == null)
                throw new Exception("Matched case wasn't found");
            return @case.Value;
        }
    }
}