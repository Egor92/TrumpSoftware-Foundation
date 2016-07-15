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
    public class NavigatingEventArgs
    {
        public NavigatingEventArgs(object nextViewModel, FrameworkElement nextView, object arg)
        {
            if (nextViewModel == null)
                throw new ArgumentNullException("nextViewModel");
            if (nextView == null)
                throw new ArgumentNullException("nextView");
            if (arg == null)
                throw new ArgumentNullException("arg");

            NextViewModel = nextViewModel;
            NextView = nextView;
            Arg = arg;
        }

        public object NextViewModel { get; private set; }
        public FrameworkElement NextView { get; private set; }
        public object Arg { get; private set; }
        public bool IsCanceled { get; private set; }

        public void Cancel()
        {
            IsCanceled = true;
        }
    }
}
