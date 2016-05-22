using System;
using System.Collections.Generic;
using System.Linq;
#if WPF
using System.Windows;
using System.Windows.Controls;
#elif WINRT
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif
using TrumpSoftware.Xaml.Mvvm;
using TrumpSoftware.Xaml.Mvvm.Interfaces;
using TrumpSoftware.Xaml.Navigation;

#if WPF
namespace TrumpSoftware.Wpf.Navigation
#elif WINRT
namespace TrumpSoftware.WinRT.Navigation
#endif
{
    public class NavigationHost : INavigationHost
    {
        #region Fields

        private readonly ContentControl _hostControl;
        private readonly IDictionary<Type, Func<FrameworkElement>> _pageFuncsByViewModelType = new Dictionary<Type, Func<FrameworkElement>>();
        private readonly IList<object> _history = new List<object>();
        private object _currentPageVM;
        private object _previousPageVM;
        private int _currentPageIndex = -1;
        private FrameworkElement _currentPage;

        #endregion

        #region CanGoBack

        public bool CanGoBack
        {
            get { return _currentPageIndex > 0; }
        }

        #endregion

        #region CanGoForward

        public bool CanGoForward
        {
            get { return _currentPageIndex < _history.Count - 1; }
        }

        #endregion

        #region Navigated

        public event EventHandler<NavigatedEventArgs> Navigated;

        protected void RaiseNavigated(object pageVM, object page)
        {
            var handler = Navigated;
            if (handler != null)
                handler(this, new NavigatedEventArgs(pageVM, page));
        }

        #endregion

        #region Ctor

        public NavigationHost(ContentControl hostControl)
        {
            if (hostControl == null)
                throw new ArgumentNullException("hostControl");
            _hostControl = hostControl;
        }

        #endregion

        public void Register<TPageVM>(Func<FrameworkElement> getPageFunc)
        {
            if (getPageFunc == null)
                throw new ArgumentNullException("getPageFunc");
            if (_pageFuncsByViewModelType.ContainsKey(typeof(TPageVM)))
                throw new Exception(string.Format("ViewModel of type {0} has been registered", typeof(TPageVM).FullName));
            _pageFuncsByViewModelType.Add(typeof(TPageVM), getPageFunc);
        }

        public void Register<TPageVM>(FrameworkElement page)
        {
            Register<TPageVM>(() => page);
        }

        public void Navigate<TPageVM>(TPageVM pageVM)
        {
            Navigate(pageVM, true);
        }

        public void RefreshPage()
        {
            if (_currentPageIndex == -1)
                return;
            _history.RemoveAt(_currentPageIndex);
            _currentPageIndex--;
            Navigate(_currentPageVM, true);
        }

        private void Navigate<TPageVM>(TPageVM pageVM, bool toRememberInHistory)
        {
            if (pageVM == null)
                throw new ArgumentNullException("pageVM");
            var navigatingPageVMType = pageVM.GetType();
            if (!_pageFuncsByViewModelType.ContainsKey(navigatingPageVMType))
                throw new Exception(string.Format("ViewModel of type {0} hasn't been registered", navigatingPageVMType.FullName));
            if (toRememberInHistory)
                RememberInHistory(pageVM);
            _currentPage = _pageFuncsByViewModelType[navigatingPageVMType].Invoke();
#if WPF
            _hostControl.Dispatcher.Invoke(() =>
#elif WINRT
            _hostControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
#endif
            {
                var navigationAware = _currentPage as INavigationAware;
                if (navigationAware != null)
                {
                    navigationAware.OnNavigatedFrom(pageVM);
                }

                navigationAware = _currentPageVM as INavigationAware;
                if (navigationAware != null)
                {
                    navigationAware.OnNavigatedFrom(pageVM);
                }

                if (_currentPageVM != null)
                {
                    _previousPageVM = _currentPageVM;
                }

                if (_currentPage != null)
                {
                    _currentPage.DataContext = pageVM;
                }

                _currentPageVM = pageVM;
                _hostControl.Content = _currentPage;

                navigationAware = _currentPageVM as INavigationAware;
                if (navigationAware != null)
                {
                    navigationAware.OnNavigatedTo(_previousPageVM);
                }

                navigationAware = _currentPage as INavigationAware;
                if (navigationAware != null)
                {
                    navigationAware.OnNavigatedTo(_previousPageVM);
                }

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
            Navigate(nextPageVM, false);
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
            Navigate(firstPageVM, false);
        }

        private void RememberInHistory<TPageVM>(TPageVM pageVM)
        {
            for (int i = _history.Count - 1; i > _currentPageIndex; i--)
                _history.RemoveAt(i);
            _history.Add(pageVM);
            _currentPageIndex++;
        }
    }
}
