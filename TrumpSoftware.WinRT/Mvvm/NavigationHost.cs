using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TrumpSoftware.Xaml.Mvvm;

namespace TrumpSoftware.WinRT.Mvvm
{
    public class NavigationHost : INavigationHost
    {
        private PageViewModel _currentPageVM;
        private PageViewModel _previousPageVM;
        private readonly Frame _frame;
        private readonly IDictionary<Type, Type> _pageTypes = new Dictionary<Type, Type>();
        private readonly IDictionary<Type, object> _parameters = new Dictionary<Type, object>();
        private readonly IList<PageViewModel> _history = new List<PageViewModel>();
        private int _currentPageIndex = -1;

        public bool CanGoBack
        {
            get { return _currentPageIndex > 0; }
        }

        public bool CanGoForward
        {
            get { return _currentPageIndex < _history.Count - 1; }
        }

        public event EventHandler<NavigatedEventArgs> Navigated;

        public NavigationHost(Frame frame)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");
            _frame = frame;
        }

        public void Register<TPageVM, TPage>(object parameter = null)
            where TPageVM : PageViewModel
            where TPage : Page
        {
            if (_pageTypes.ContainsKey(typeof(TPageVM)))
                throw new Exception(string.Format("PageViewModel of type {0} has been registered", typeof(TPageVM).FullName));
            _pageTypes.Add(typeof(TPageVM), typeof(TPage));
            _parameters.Add(typeof(TPageVM), parameter);
        }

        public void Navigate<TPageVM>(TPageVM pageVM)
            where TPageVM : PageViewModel
        {
            Navigate(pageVM, true, false);
        }

        public void RefreshPage()
        {
            if (_currentPageVM != null)
                Navigate(_currentPageVM, false, false);
        }

        private void Navigate<TPageVM>(TPageVM pageVM, bool toRememberInHistory, bool toResetViewModel)
            where TPageVM : PageViewModel
        {
            if (pageVM == null)
                throw new ArgumentNullException("pageVM");
            var navigatingPageVMType = pageVM.GetType();
            if (!_pageTypes.ContainsKey(navigatingPageVMType))
                throw new Exception(string.Format("PageViewModel of type {0} hasn't been registered", navigatingPageVMType.FullName));
            if (toRememberInHistory)
                RememberInHistory(pageVM);
            if (toResetViewModel)
                ResetFieldsHelper.ResetFields(pageVM);
            var pageType = _pageTypes[navigatingPageVMType];
            var parameter = _parameters[navigatingPageVMType];
            _frame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (_currentPageVM != null)
                {
                    _currentPageVM.OnNavigatedFrom(pageVM);
                    _previousPageVM = _currentPageVM;
                }
                _currentPageVM = pageVM;
                _frame.Navigated += Frame_Navigated;
                _frame.Navigate(pageType, parameter);
                _frame.Navigated -= Frame_Navigated;
            });
        }

        private void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var page = e.Content as Page;
            if (page == null)
                return;
            page.DataContext = _currentPageVM;
            _currentPageVM.OnNavigatedTo(_previousPageVM);
            RaiseNavigated(_currentPageVM, page);
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
        
        private void RaiseNavigated(PageViewModel pageVM, Page page)
        {
            var handler = Navigated;
            if (handler != null)
                handler(this, new NavigatedEventArgs(pageVM, page));
        }
    }
}
