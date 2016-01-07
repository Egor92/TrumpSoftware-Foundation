#if WINRT
using System;
using System.Reflection;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters.Cases
#elif WINRT
namespace TrumpSoftware.WinRT.Converters.Cases
#endif
{
    public static class ObjectHelper
    {
        public static new bool Equals(object x, object y)
        {
            if (x == null || y == null)
                return false;
#if WINRT
            var xType = x.GetType();
            var yType = y.GetType();
            if (xType != yType)
                return false;
            if (xType.GetTypeInfo().IsEnum)
                x = Convert.ToInt32(x);
            if (yType.GetTypeInfo().IsEnum)
                y = Convert.ToInt32(x);
#endif
            return x.Equals(y);
        }
    }
}
