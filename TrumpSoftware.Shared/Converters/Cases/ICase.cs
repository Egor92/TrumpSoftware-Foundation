#if WPF
namespace TrumpSoftware.Wpf.Converters.Cases
#elif WINRT
namespace TrumpSoftware.WinRT.Converters.Cases
#endif
{
    public interface ICase
    {
        bool IsMatched(object value);
        object Value { get; }
    }
}