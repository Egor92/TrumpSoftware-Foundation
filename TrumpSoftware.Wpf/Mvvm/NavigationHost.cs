using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Navigation;
using TrumpSoftware.Xaml.Mvvm;

namespace TrumpSoftware.Wpf.Mvvm
{
    public class NavigationHost : INavigationHost
    {
        private readonly NavigationService _navigationService;
        private readonly IDictionary<Type, Page> _pages = new Dictionary<Type, Page>();
        private readonly IList<ViewModelBase> _history = new List<ViewModelBase>();
        private int _currentPageIndex = -1;

        public NavigationHost(NavigationService navigationService)
        {
            if (navigationService == null)
                throw new ArgumentNullException("navigationService");
            _navigationService = navigationService;
        }

        public void Register<TPageVM>(Page page)
            where TPageVM : ViewModelBase
        {
            if (_pages.ContainsKey(typeof(TPageVM)))
                throw new Exception(string.Format("PageViewModel of type {0} has been registered", typeof(TPageVM).FullName));
            _pages.Add(typeof (TPageVM), page);
        }

        public bool CanGoBack
        {
            get { return _currentPageIndex > 0; }
        }

        public bool CanGoForward
        {
            get { return _currentPageIndex < _history.Count - 1; }
        }

        public void Navigate<TPageVM>(TPageVM pageVM)
            where TPageVM : ViewModelBase
        {
            Navigate(pageVM, true);
        }

        private void Navigate<TPageVM>(TPageVM pageVM, bool toRememberInHistory)
            where TPageVM : ViewModelBase
        {
            var navigatingPageVMType = pageVM.GetType();
            if (!_pages.ContainsKey(navigatingPageVMType))
                throw new Exception(string.Format("PageViewModel of type {0} hasn't been registered", navigatingPageVMType.FullName));
            if (toRememberInHistory)
                RememberInHistory(pageVM);
            var page = _pages[navigatingPageVMType];
            page.DataContext = pageVM;
            _navigationService.Navigate(page, pageVM);
        }

        public void GoBack()
        {
            if (!CanGoBack)
                return;
            var previousPageVM = _history[_currentPageIndex-1];
            if (previousPageVM == null)
                return;
            var reset = previousPageVM as IReseter;
            if (reset != null)
                reset.Reset();
            Navigate(previousPageVM, false);
            _currentPageIndex--;
        }

        public void GoForward()
        {
            if (!CanGoForward)
                return;
            var nextPageVM = _history[_currentPageIndex+1];
            if (nextPageVM == null)
                return;
            var reset = nextPageVM as IReseter;
            if (reset != null)
                reset.Reset();
            Navigate(nextPageVM, false);
            _currentPageIndex++;
        }

        private void RememberInHistory<TPageVM>(TPageVM pageVM)
            where TPageVM : ViewModelBase
        {
            for (int i = _history.Count - 1; i > _currentPageIndex; i--)
                _history.RemoveAt(i);
            _history.Add(pageVM);
            _currentPageIndex++;
        }
    }
}
