using System;
using System.Globalization;
using System.Reflection;

namespace TrumpSoftware.Common
{
    public static class ConvertEx
    {
        public static bool TryConvert<T>(object value, IFormatProvider formatProvider, out T result)
        {
            Type targetType = typeof(T);

            if (value is T)
            {
                result = (T)value;
                return true;
            }

            var typeInfo = targetType.GetTypeInfo();
            if (value == null && !typeInfo.IsValueType)
            {
                result = (T)value;
                return true;
            }

            var nullableUnderlyingType = Nullable.GetUnderlyingType(targetType);

            if (value == null && nullableUnderlyingType != null)
            {
                result = (T)value;
                return true;
            }

            if (nullableUnderlyingType != null)
            {
                targetType = nullableUnderlyingType;
                typeInfo = nullableUnderlyingType.GetTypeInfo();
            }

            if (value == null && typeInfo.IsValueType)
            {
                result = default(T);
                return false;
            }

            if (!typeInfo.IsPrimitive && targetType != typeof(string))
            {
                result = default(T);
                return false;
            }

            try
            {
                result = (T)System.Convert.ChangeType(value, targetType, formatProvider);
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }
            return true;
        }

        public static bool TryConvert<T>(object value, out T result)
        {
            return TryConvert(value, CultureInfo.CurrentCulture, out result);
        }

        public static T Convert<T>(object value, IFormatProvider formatProvider)
        {
            T result;
            if (!TryConvert<T>(value, formatProvider, out result))
                throw new Exception(string.Format("Can not convert {0} to type '{1}'", GetValueTypeString(value), typeof(T)));
            return result;
        }

        public static T Convert<T>(object value)
        {
            return Convert<T>(value, CultureInfo.CurrentCulture);
        }

        private static string GetValueTypeString(object value)
        {
            if (value == null)
                return "'null'";
            return string.Format("value of type '{0}'", value.GetType());
        }
    }
}
