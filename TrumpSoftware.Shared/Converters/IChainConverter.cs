using System;
using TrumpSoftware.Common;

#if WPF
using System.Windows.Data;
using TrumpSoftware.Wpf.Interfaces;
#elif WINRT
using Windows.UI.Xaml.Data;
using TrumpSoftware.WinRT.Interfaces;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public interface IChainConverter : IValueConverter, IHaveConverter
    {
    }

    public static class ChainConverterExtensions
    {
        public static T CastValue<T>(this IChainConverter chainConverter, object value)
        {
            T result;
            if (!ConvertEx.TryConvert(value, out result))
            {
                var message = string.Format("Can not convert {0} to type '{1}' in converter of type '{2}'", GetValueTypeString(value), typeof(T), chainConverter.GetType());
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
