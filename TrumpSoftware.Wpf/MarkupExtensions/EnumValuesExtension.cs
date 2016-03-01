using System;
using System.Linq;
using System.Windows.Markup;
using TrumpSoftware.Common.Exceptions;

namespace TrumpSoftware.Wpf.MarkupExtensions
{
    public enum NullValueInclusion
    {
        NoNullValue,
        IncludeFirst,
        IncludeLast,
    }

    public class EnumValuesExtension : MarkupExtension
    {
        #region Fields

        private readonly Type _enumType;

        #endregion

        #region Ctor

        public EnumValuesExtension(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum)
                throw new NotSupportedException(string.Format("{0} is not enum type", _enumType.FullName));
            _enumType = enumType;
            NullValueInclusion = NullValueInclusion.NoNullValue;
        }

        #endregion

        public NullValueInclusion NullValueInclusion { get; set; }

        #region Overridden members

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = _enumType.GetEnumValues();
            switch (NullValueInclusion)
            {
                case NullValueInclusion.NoNullValue:
                    return enumValues;
                case NullValueInclusion.IncludeFirst:
                    return new object[] { null }.Union(enumValues.Cast<object>()).ToArray();
                case NullValueInclusion.IncludeLast:
                    return enumValues.Cast<object>().Union(new object[] { null }).ToArray();
                default:
                    throw new UnhandledCaseException(NullValueInclusion);
            }
        }

        #endregion
    }
}
