using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml;

namespace TrumpSoftware.WinRT.Converters
{
    public class VisibilityConverter : ChainConverter
    {
        public IList<object> Values { get; set; }

        public Visibility ValueContainsVisibility { get; set; }

        public VisibilityConverter()
        {
            Values = new List<object>();
            ValueContainsVisibility = Visibility.Visible;
        }

        protected override object Convert(object value, Type targetType, object parameter, string language)
        {
            if (Values == null)
                return ValueContainsVisibility.GetNegation();
            foreach (var val in Values)
            {
                if (value is Enum)
                {
                    if ((int)value == (int)val)
                        return ValueContainsVisibility;
                }
                else if (Equals(value, val))
                    return ValueContainsVisibility;
            }
            return ValueContainsVisibility.GetNegation();
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
