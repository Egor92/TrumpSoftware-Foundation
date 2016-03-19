using System;
#if WPF
using System.Windows.Media;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using CultureArgumentType = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public class StringToGeometryConverter : ChainConverter<string, Geometry>
    {
        protected override Geometry Convert(string value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            try
            {
                return ParseGeometry(value);
            }
            catch (Exception)
            {
                return Geometry.Empty;
            }
        }

        protected override string ConvertBack(Geometry value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            throw new NotSupportedException();
        }

#if WPF
        private static Geometry ParseGeometry(string source)
        {
            return Geometry.Parse(source);
        }
#elif WINRT
        private static Geometry ParseGeometry(string source)
        {
            var xamlSource = string.Format("<Path xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Data='{0}' />", source);
            var path = (Path)XamlReader.Load(xamlSource);
            return path.Data;
        }
#endif
    }
}
