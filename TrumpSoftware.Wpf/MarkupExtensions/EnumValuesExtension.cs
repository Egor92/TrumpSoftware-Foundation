using System;
using System.Windows.Markup;

namespace TrumpSoftware.Wpf.MarkupExtensions
{
    public class EnumValuesExtension : MarkupExtension
    {
        private readonly Type _enumType;

        public EnumValuesExtension(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum)
                throw new NotSupportedException(string.Format("{0} is not enum type", _enumType.FullName));
            _enumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _enumType.GetEnumValues();
        }
    }
}
