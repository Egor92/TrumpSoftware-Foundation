namespace TrumpSoftware.Xaml.Mvvm
{
    public abstract class PageViewModel : NotificationObject
    {
        public virtual void OnNavigatedTo(PageViewModel previousPageVM)
        {
            
        }

        public virtual void OnNavigatedFrom(PageViewModel nextPageVM)
        {
            
        }
    }
}
