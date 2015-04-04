using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using TrumpSoftware.WPF.Converters;

namespace TrumpSoftware.Wpf.Converters
{
    public class Case : DependencyObject
    {
        public Type Type { get; set; }

        public object Key { get; set; }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof (object), typeof (Case));

        public object Value
        {
            get { return (object) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }

    [ContentProperty("Cases")]
    public class SwitchConverter : ChainConverter<object,object>
    {
        public IList<Case> Cases { get; set; }

        public object Default { get; set; }

        public SwitchConverter()
        {
            Cases = new List<Case>();
        }

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var @case in Cases)
            {
                if (@case.Type == null && @case.Key == null)
                    throw new Exception("Type or Key must be not null");
                if (@case.Type == null || @case.Type.IsInstanceOfType(value))
                {
                    if ((@case.Key == null && value == null) || Equals(@case.Key, value))
                        return @case.Value;
                }
            }
            return Default;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
