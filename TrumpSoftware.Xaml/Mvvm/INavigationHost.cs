namespace TrumpSoftware.Xaml.Mvvm
{
    public interface INavigationHost
    {
        bool CanGoBack { get; }

        bool CanGoForward { get; }

        void Navigate<TPageVM>(TPageVM pageVM)
            where TPageVM : ViewModelBase;

        void RefreshPage();

        void GoBack();

        void GoForward();
    }
}