using System;
using Windows.UI.Core;
using TrumpSoftware.Xaml.Interfaces;

namespace TrumpSoftware.WinRT.PlatformSpecifics
{
    public class UIThread : IUIThread
    {
        public void Invoke(Action action)
        {
            var coreWindow = CoreWindow.GetForCurrentThread();
            coreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }
    }
}
