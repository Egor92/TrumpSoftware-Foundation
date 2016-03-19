#if WPF
using System.Windows.Data;
using TrumpSoftware.Wpf.Interfaces;
#elif WINRT
using System.Windows.Data;
using TrumpSoftware.WinRT.Interfaces;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public interface IChainMultiConverter : IHaveConverter, IMultiValueConverter
    {
    }
}
