using System;

#if WPF
using System.Windows;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using Windows.UI.Xaml;
using CultureArgumentType = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public class GetXamlResourceConverter : ChainConverter<object, object>
    {
        public FrameworkElement ResourceOwner { get; set; }

        protected override object Convert(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            if (value == null)
                return null;

            var resources = ResourceOwner != null
                ? ResourceOwner.Resources
                : Application.Current.Resources;

#if WPF
            if (!resources.Contains(value))
                return null;
            return resources[value];
#elif WINRT
            object result;
            resources.TryGetValue(value, out result);
            return result;
#endif
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            throw new NotSupportedException();
        }
    }
}
