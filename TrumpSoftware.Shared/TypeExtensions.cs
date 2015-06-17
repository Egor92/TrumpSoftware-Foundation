using System;
#if WINRT
using System.Reflection;
#endif

#if WPF
namespace TrumpSoftware.Wpf
#elif WINRT
namespace TrumpSoftware.WinRT
#endif
{
    public static class TypeExtensions
    {
#if WINRT
        public static bool IsAssignableFrom(this Type sourceType, Type targetType)
        {
            var sourceTypeInfo = sourceType.GetTypeInfo();
            var targetTypeInfo = targetType.GetTypeInfo();
            return sourceTypeInfo.IsAssignableFrom(targetTypeInfo);
        }

        public static bool IsInstanceOfType(this Type type, object obj)
        {
            var sourceType = obj.GetType();
            return IsAssignableFrom(sourceType, type);
        }
#endif

        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

#if WPF
        private static Type GetTypeInfo(this Type type)
        {
            return type;
        }
#endif
    }
}
