using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TrumpSoftware.WinRT
{
    public class OrientationAwarePage : Page
    {
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            WindowOrientationObserver.AddSubscriber(this);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            WindowOrientationObserver.RemoveSubscriber(this);
        }
    }
}
