using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TrumpSoftware.Xaml.Mvvm;

namespace TrumpSoftware.Wpf.Mvvm
{
    public class NavigationHost : INavigationHost
    {
        private readonly ContentControl _hostControl;
        private readonly IDictionary<Type, FrameworkElement> _pages = new Dictionary<Type, FrameworkElement>();
        private readonly IList<PageViewModel> _history = new List<PageViewModel>();
        private PageViewModel _currentPageVM;
        private PageViewModel _previousPageVM;
        private int _currentPageIndex = -1;
        private FrameworkElement _currentPage;

        public bool CanGoBack
        {
            get { return _currentPageIndex > 0; }
        }

        public bool CanGoForward
        {
            get { return _currentPageIndex < _history.Count - 1; }
        }

        public event EventHandler<NavigatedEventArgs> Navigated;

        public NavigationHost(ContentControl hostControl)
        {
            if (hostControl == null)
                throw new ArgumentNullException("hostControl");
            _hostControl = hostControl;
        }

        public void Register<TPageVM>(FrameworkElement page)
            where TPageVM : PageViewModel
        {
            if (_pages.ContainsKey(typeof(TPageVM)))
                throw new Exception(string.Format("PageViewModel of type {0} has been registered", typeof(TPageVM).FullName));
            _pages.Add(typeof (TPageVM), page);
        }

        public void Navigate<TPageVM>(TPageVM pageVM)
            where TPageVM : PageViewModel
        {
            Navigate(pageVM, true, false);
        }

        public void RefreshPage()
        {
            _currentPage.DataContext = null;
            _currentPage.DataContext = _currentPageVM;
        }

        private void Navigate<TPageVM>(TPageVM pageVM, bool toRememberInHistory, bool toResetViewModel)
            where TPageVM : PageViewModel
        {
            if (pageVM == null)
                throw new ArgumentNullException("pageVM");
            var navigatingPageVMType = pageVM.GetType();
            if (!_pages.ContainsKey(navigatingPageVMType))
                throw new Exception(string.Format("PageViewModel of type {0} hasn't been registered", navigatingPageVMType.FullName));
            if (toRememberInHistory)
                RememberInHistory(pageVM);
            if (toResetViewModel)
                ResetFieldsHelper.ResetFields(pageVM);
            _currentPage = _pages[navigatingPageVMType];
            _hostControl.Dispatcher.Invoke(() =>
            {
                if (_currentPageVM != null)
                {
                    _currentPageVM.OnNavigatedFrom(pageVM);
                    _previousPageVM = pageVM;
                }
                _currentPage.DataContext = null;
                _currentPage.DataContext = pageVM;
                _currentPageVM = pageVM;
                _hostControl.Content = _currentPage;
                pageVM.OnNavigatedTo(_previousPageVM);
                RaiseNavigated(_currentPageVM, _currentPage);
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

        public void GoHome()
        {
            if (_currentPageIndex == -1)
                throw new Exception("There is no first page to navigate home");
            var firstPageVM = _history.First();
            _history.Clear();
            _history.Add(firstPageVM);
            if (_currentPageIndex == 0)
                return;
            _currentPageIndex = 0;
            Navigate(firstPageVM, false, false);
        }

        private void RememberInHistory<TPageVM>(TPageVM pageVM)
            where TPageVM : PageViewModel
        {
            for (int i = _history.Count - 1; i > _currentPageIndex; i--)
                _history.RemoveAt(i);
            _history.Add(pageVM);
            _currentPageIndex++;
        }

        protected void RaiseNavigated(PageViewModel pageVM, object page)
        {
            var handler = Navigated;
            if (handler != null)
                handler(this, new NavigatedEventArgs(pageVM, page));
        }
    }
}
