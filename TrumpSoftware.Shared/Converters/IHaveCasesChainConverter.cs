using System.Collections.Generic;

#if WPF
using TrumpSoftware.Wpf.Converters.Cases;
#elif WINRT
using TrumpSoftware.WinRT.Converters.Cases;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public interface IHaveCasesChainConverter : IChainConverter
    {
        List<ICase> Cases { get; }
    }
}
