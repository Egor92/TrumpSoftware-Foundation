namespace TrumpSoftware.Xaml.Navigation
{
    public interface INavigationAware
    {
        void OnNavigatedTo(NavigationParameters parameters);

        void OnNavigatingFrom(NavigationParameters parameters);
    }
}
