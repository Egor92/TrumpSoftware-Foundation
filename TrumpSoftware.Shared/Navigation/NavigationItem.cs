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
    public class NavigationItem
    {
        public NavigationItem(object viewModel, FrameworkElement view)
        {
            ViewModel = viewModel;
            View = view;
        }

        public object ViewModel { get; private set; }
        public FrameworkElement View { get; private set; }
    }
}
