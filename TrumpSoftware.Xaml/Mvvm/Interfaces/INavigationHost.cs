using System;

namespace TrumpSoftware.Xaml.Mvvm.Interfaces
{
    public interface INavigationHost
    {
        bool CanGoBack { get; }

        bool CanGoForward { get; }

        event EventHandler<NavigatedEventArgs> Navigated;

        void Navigate<TPageVM>(TPageVM pageVM);

        void RefreshPage();

        void GoBack();

        void GoForward();

        void GoHome();
    }
}