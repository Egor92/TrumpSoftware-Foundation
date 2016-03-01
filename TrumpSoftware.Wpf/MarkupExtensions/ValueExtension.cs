using System;
using System.Globalization;
using System.Windows.Markup;
using TrumpSoftware.Common;

namespace TrumpSoftware.Wpf.MarkupExtensions
{
    public class ValueExtension : MarkupExtension
    {
        #region Fields

        private readonly object _value;
        private readonly Type _type;

        #endregion

        #region Ctor

        public ValueExtension(object value, Type type)
        {
            _value = value;
            _type = type;
        }

        #endregion

        #region Properties

        #region FormatProvider

        public IFormatProvider FormatProvider { get; set; }

        #endregion

        #endregion

        #region Overridden members

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var formatProvider = FormatProvider ?? CultureInfo.InvariantCulture;
            return ConvertEx.Convert(_value, _type, formatProvider);
        }

        #endregion
    }
}
