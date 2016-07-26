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

        public static Result<object> TryConvert(object value, Type targetType, IFormatProvider formatProvider)
        {
            var targetTypeInfo = targetType.GetTypeInfo();

            if (value != null && targetTypeInfo.IsAssignableFrom(value.GetType().GetTypeInfo()))
            {
                return new Result<object>(true, value);
            }

            if (value == null && !targetTypeInfo.IsValueType)
            {
                return new Result<object>(true, null);
            }

            var nullableUnderlyingType = Nullable.GetUnderlyingType(targetType);

            if (value == null && nullableUnderlyingType != null)
            {
                return new Result<object>(true, null);
            }

            if (nullableUnderlyingType != null)
            {
                targetType = nullableUnderlyingType;
                targetTypeInfo = nullableUnderlyingType.GetTypeInfo();
            }

            if (value == null && targetTypeInfo.IsValueType)
            {
                var result = targetType.GetDefault();
                return new Result<object>(false, result);
            }

            if (!targetTypeInfo.IsPrimitive && targetType != typeof (string))
            {
                var result = targetType.GetDefault();
                return new Result<object>(false, result);
            }

            try
            {
                var result = System.Convert.ChangeType(value, targetType, formatProvider);
                return new Result<object>(true, result);
            }
            catch (Exception)
            {
                var result = targetType.GetDefault();
                return new Result<object>(false, result);
            }
        }

        public static Result<object> TryConvert(object value, Type targetType)
        {
            return TryConvert(value, targetType, DefaultFormatProvider);
        }

        public static Result<T> TryConvert<T>(object value, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
                throw new ArgumentNullException(nameof(formatProvider));
            var targetType = typeof(T);
            if (targetType == null)
                throw new ArgumentNullException("T");
            Result<object> convertResult = TryConvert(value, targetType, formatProvider);
            return (Result<T>)convertResult;
        }

        public static Result<T> TryConvert<T>(object value)
        {
            return TryConvert<T>(value, DefaultFormatProvider);
        }

        public static object Convert(object value, Type targetType, IFormatProvider formatProvider)
        {
            var convertResult = TryConvert(value, targetType, formatProvider);
            if (!convertResult.IsSuccess)
                throw new Exception(string.Format("Can not convert {0} to type '{1}'", GetValueTypeString(value), targetType));
            return convertResult.Data;
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