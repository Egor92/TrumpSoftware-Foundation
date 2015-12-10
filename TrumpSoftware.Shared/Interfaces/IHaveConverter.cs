#if WPF
using System.Windows.Data;
#elif WINRT
using Windows.UI.Xaml.Data;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Interfaces
#elif WINRT
namespace TrumpSoftware.WinRT.Interfaces
#endif
{
    public interface IHaveConverter
    {
        IValueConverter Converter { get; }
    }
}
