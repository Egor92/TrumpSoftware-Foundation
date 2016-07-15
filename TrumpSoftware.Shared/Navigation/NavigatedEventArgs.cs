using System;
#if WPF
using System.Windows;
#elif WINRT
using Windows.UI.Xaml;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Navigation
#elif WINRT
namespace TrumpSoftware.WinRT.Navigation
#endif
{
    public class NavigatedEventArgs : EventArgs
    {
        public NavigatedEventArgs(object prevViewModel, FrameworkElement prevView, object arg)
        {
            if (prevViewModel == null)
                throw new ArgumentNullException("prevViewModel");
            if (prevView == null)
                throw new ArgumentNullException("prevView");
            if (arg == null)
                throw new ArgumentNullException("arg");

            PrevViewModel = prevViewModel;
            PrevView = prevView;
            Arg = arg;
        }

        public object PrevViewModel { get; private set; }
        public FrameworkElement PrevView { get; private set; }
        public object Arg { get; private set; }
    }
}
