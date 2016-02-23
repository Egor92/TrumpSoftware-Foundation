using System;
using TrumpSoftware.Common;

#if WPF
namespace TrumpSoftware.Wpf.Converters.Helpers
#elif WINRT
namespace TrumpSoftware.WinRT.Converters.Helpers
#endif
{
    public static class ChainConverterHelper
    {
        public static T CastValue<T>(object value, object converter)
        {
            T result;
            if (!ConvertEx.TryConvert(value, out result))
            {
                var message = string.Format("Can not convert {0} to type '{1}' in converter of type '{2}'", GetValueTypeString(value), typeof(T), converter.GetType());
                throw new Exception(message);
            }
            return result;
        }

        private static string GetValueTypeString(object value)
        {
            if (value == null)
                return "'null'";
            return string.Format("value of type '{0}'", value.GetType());
        }
    }
}
