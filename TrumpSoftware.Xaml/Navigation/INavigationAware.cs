namespace TrumpSoftware.Xaml.Navigation
{
    public interface INavigationAware
    {
        void OnNavigatedTo(object previousPageVM);

        void OnNavigatedFrom(object nextPageVM);
    }
}
