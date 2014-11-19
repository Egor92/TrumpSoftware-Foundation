namespace TrumpSoftware.Mvvm
{
    public interface INavigationHost
    {
        bool CanGoBack { get; }

        bool CanGoForward { get; }

        void Navigate<TPageVM>(TPageVM pageVM)
            where TPageVM : ViewModelBase;

        void GoBack();

        void GoForward();
    }
}