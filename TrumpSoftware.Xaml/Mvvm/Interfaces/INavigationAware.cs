namespace TrumpSoftware.Xaml.Mvvm.Interfaces
{
    public interface INavigationAware
    {
        void OnNavigatedTo(object previousPageVM);

        void OnNavigatedFrom(object nextPageVM);
    }
}
