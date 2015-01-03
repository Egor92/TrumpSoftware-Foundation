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
        private readonly IList<PageViewModel> _history = new List<PageViewModel>();
        private PageViewModel _currentPageVM;
        private int _currentPageIndex = -1;

        public NavigationHost(NavigationService navigationService)
        {
            if (navigationService == null)
                throw new ArgumentNullException("navigationService");
            _navigationService = navigationService;
        }

        public void Register<TPageVM>(Page page)
            where TPageVM : PageViewModel
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
            where TPageVM : PageViewModel
        {
            Navigate(pageVM, true, false);
        }

        public void RefreshPage()
        {
            _navigationService.Refresh();
        }

        private void Navigate<TPageVM>(TPageVM pageVM, bool toRememberInHistory, bool toResetViewModel)
            where TPageVM : PageViewModel
        {
            var navigatingPageVMType = pageVM.GetType();
            if (!_pages.ContainsKey(navigatingPageVMType))
                throw new Exception(string.Format("PageViewModel of type {0} hasn't been registered", navigatingPageVMType.FullName));
            if (toRememberInHistory)
                RememberInHistory(pageVM);
            if (toResetViewModel)
                ViewModelResetHelper.ResetFields(pageVM);
            var page = _pages[navigatingPageVMType];
            page.Dispatcher.Invoke(() =>
            {
                if (_currentPageVM != null)
                    _currentPageVM.OnNavigatedFrom();
                page.DataContext = null;
                page.DataContext = pageVM;
                _currentPageVM = pageVM;
                _navigationService.Navigate(page);
                pageVM.OnNavigatedTo();
            });
        }

        public void GoBack()
        {
            if (!CanGoBack)
                return;
            var previousPageVM = _history[_currentPageIndex-1];
            if (previousPageVM == null)
                return;
            Navigate(previousPageVM, false, true);
            _currentPageIndex--;
        }

        public void GoForward()
        {
            if (!CanGoForward)
                return;
            var nextPageVM = _history[_currentPageIndex+1];
            if (nextPageVM == null)
                return;
            Navigate(nextPageVM, false, true);
            _currentPageIndex++;
        }

        private void RememberInHistory<TPageVM>(TPageVM pageVM)
            where TPageVM : PageViewModel
        {
            for (int i = _history.Count - 1; i > _currentPageIndex; i--)
                _history.RemoveAt(i);
            _history.Add(pageVM);
            _currentPageIndex++;
        }
    }
}
