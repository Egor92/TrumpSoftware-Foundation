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
    internal class NavigationItem
    {
        public NavigationItem(object viewModel, FrameworkElement view)
        {
            ViewModel = viewModel;
            View = view;
        }

        internal object ViewModel { get; private set; }
        internal FrameworkElement View { get; private set; }
    }
}
