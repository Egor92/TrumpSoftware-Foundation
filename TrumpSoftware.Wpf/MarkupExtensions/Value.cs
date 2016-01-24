using System;
using System.Windows.Markup;
using TrumpSoftware.Common;

namespace TrumpSoftware.Wpf.MarkupExtensions
{
    public class Value : MarkupExtension
    {
        #region Fields

        private readonly object _value;
        private readonly Type _type;

        #endregion

        #region Ctor

        public Value(object value, Type type)
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
            return ConvertEx.Convert(_value, _type, FormatProvider);
        }

        #endregion
    }
}
