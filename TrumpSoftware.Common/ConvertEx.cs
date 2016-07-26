using System;
using System.Globalization;
using System.Reflection;
using TrumpSoftware.Common.Extensions;

namespace TrumpSoftware.Common
{
    public static class ConvertEx
    {
        private static IFormatProvider DefaultFormatProvider
        {
            get { return CultureInfo.InvariantCulture; }
        }

        public static bool TryConvert(object value, Type targetType, IFormatProvider formatProvider, out object result)
        {
            var targetTypeInfo = targetType.GetTypeInfo();

            if (value != null && targetTypeInfo.IsAssignableFrom(value.GetType().GetTypeInfo()))
            {
                result = value;
                return true;
            }

            if (value == null && !targetTypeInfo.IsValueType)
            {
                result = null;
                return true;
            }

            var nullableUnderlyingType = Nullable.GetUnderlyingType(targetType);

            if (value == null && nullableUnderlyingType != null)
            {
                result = null;
                return true;
            }

            if (nullableUnderlyingType != null)
            {
                targetType = nullableUnderlyingType;
                targetTypeInfo = nullableUnderlyingType.GetTypeInfo();
            }

            if (value == null && targetTypeInfo.IsValueType)
            {
                result = targetType.GetDefault();
                return false;
            }

            if (!targetTypeInfo.IsPrimitive && targetType != typeof (string))
            {
                result = targetType.GetDefault();
                return false;
            }

            try
            {
                result = System.Convert.ChangeType(value, targetType, formatProvider);
            }
            catch (Exception)
            {
                result = targetType.GetDefault();
                return false;
            }
            return true;
        }

        public static bool TryConvert(object value, Type targetType, out object result)
        {
            return TryConvert(value, targetType, DefaultFormatProvider, out result);
        }

        public static bool TryConvert<T>(object value, IFormatProvider formatProvider, out T result)
        {
            object internalResult;
            bool isConvertedSuccessfully = TryConvert(value, typeof(T), formatProvider, out internalResult);
            result = (T)internalResult;
            return isConvertedSuccessfully;
        }

        public static bool TryConvert<T>(object value, out T result)
        {
            return TryConvert(value, DefaultFormatProvider, out result);
        }

        public static object Convert(object value, Type targetType, IFormatProvider formatProvider)
        {
            object result;
            if (!TryConvert(value, targetType, formatProvider, out result))
                throw new Exception(string.Format("Can not convert {0} to type '{1}'", GetValueTypeString(value), targetType));
            return result;
        }

        public static object Convert(object value, Type targetType)
        {
            return Convert(value, targetType, DefaultFormatProvider);
        }

        public static T Convert<T>(object value, IFormatProvider formatProvider)
        {
            return (T)Convert(value, typeof (T), formatProvider);
        }

        public static T Convert<T>(object value)
        {
            return Convert<T>(value, DefaultFormatProvider);
        }

        private static string GetValueTypeString(object value)
        {
            if (value == null)
                return "'null'";
            return string.Format("value of type '{0}'", value.GetType());
        }
    }
}