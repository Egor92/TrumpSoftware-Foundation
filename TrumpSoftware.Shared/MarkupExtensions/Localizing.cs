using TrumpSoftware.Common.Extensions;
using TrumpSoftware.Common.Enums;
using System;
#if WPF
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using CultureArgumentType = System.String;
using Windows.UI.Xaml.Data;
#endif

#if WPF
namespace TrumpSoftware.Wpf.MarkupExtensions
#elif WINRT
namespace TrumpSoftware.WinRT.MarkupExtensions
#endif
{
    public class Localizing : Binding, IValueConverter
    {
        #region WordCase

        public WordCase WordCase { get; set; }

        #endregion

        #region Ctor

        public Localizing()
        {
            Source = this;
            Converter = this;
            Mode = BindingMode.OneWay;
        }

        public Localizing(object source) // set Source to null for using DataContext
            : this()
        {
            Source = source;
        }

        public Localizing(RelativeSource relativeSource)
        {
            RelativeSource = relativeSource;
            Converter = this;
        }

        #endregion

        #region Overridden methods

        public object Convert(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            var stringValue = value as string;
            if (stringValue == null)
                return value;
            return stringValue.TransformToCase(WordCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string cultureArgument)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
