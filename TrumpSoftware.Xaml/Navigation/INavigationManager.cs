namespace TrumpSoftware.Xaml.Navigation
{
    public interface INavigationManager
    {
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        void Navigate<TViewModel>(object arg);
        void GoBack();
        void GoForward();
    }
}