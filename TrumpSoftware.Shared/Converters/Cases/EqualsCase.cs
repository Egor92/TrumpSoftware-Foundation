#if WPF
namespace TrumpSoftware.Wpf.Converters.Cases
#elif WINRT
namespace TrumpSoftware.WinRT.Converters.Cases
#endif
{
    public class EqualsCase : ICase
    {
        public object Key { get; set; }

        public object Value { get; set; }

        public bool IsMatched(object value)
        {
            return ObjectHelper.Equals(value, Key);
        }
    }
}
