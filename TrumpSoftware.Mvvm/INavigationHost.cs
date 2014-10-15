namespace TrumpSoftware.Mvvm
{
    public interface INavigationHost
    {
        void Navigate<TPageVM>(TPageVM pageVM)
            where TPageVM : PageViewModel;

        bool CanGoBack { get; }

        void GoBack();
    }
}