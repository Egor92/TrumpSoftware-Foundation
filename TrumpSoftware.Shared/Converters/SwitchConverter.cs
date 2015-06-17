using System;
using System.Collections.Generic;

#if WPF
using System.Windows;
using System.Windows.Markup;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using CultureArgumentType = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public class Case : DependencyObject
    {
        public Type Type { get; set; }

        public object Key { get; set; }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof (object), typeof (Case), new PropertyMetadata(null));

        public object Value
        {
            get { return (object) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }

#if WPF
    [ContentProperty("Cases")]
#elif WINRT
    [ContentProperty(Name = "Cases")]
#endif
    public class SwitchConverter : ChainConverter<object,object>
    {
        public IList<Case> Cases { get; set; }

        public object Default { get; set; }

        public SwitchConverter()
        {
            Cases = new List<Case>();
        }

        protected override object Convert(object value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            foreach (var @case in Cases)
            {
                if (@case.Type == null && @case.Key == null)
                    throw new Exception("Type or Key must be not null");
                bool caseResult = true;
                if (@case.Type != null)
                    caseResult &= @case.Type.IsInstanceOfType(value);
                if (@case.Key != null)
                    caseResult &= Equals(@case.Key, value);
                if (caseResult)
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
