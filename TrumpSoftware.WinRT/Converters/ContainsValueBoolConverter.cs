using System;
using System.Collections.Generic;

namespace TrumpSoftware.WinRT.Converters
{
    public class ContainsValueBoolConverter : ChainConverter
    {
        public IList<object> Values { get; set; }

        public ContainsValueBoolConverter()
        {
            Values = new List<object>();
        }

        protected override object Convert(object value, Type targetType, object parameter, string language)
        {
            if (Values == null)
                return false;
            foreach (var val in Values)
            {
                if (value is Enum)
                {
                    if ((int)value == (int)val)
                        return true;
                }
                else if (Equals(value, val))
                    return true;
            }
            return false;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
