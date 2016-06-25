using System;
using System.ComponentModel;
using System.Windows.Markup;

namespace TrumpSoftware.Wpf.MarkupExtensions
{
    public class ValueExtension : MarkupExtension
    {
        #region Fields

        private readonly string _value;
        private readonly Type _type;

        #endregion

        #region Ctor

        public ValueExtension(string value, Type type)
        {
            _value = value;
            _type = type;
        }

        #endregion

        #region Overridden members

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var typeConverter = TypeDescriptor.GetConverter(_type);
            return typeConverter.ConvertFromString(_value);
        }

        #endregion
    }
}
