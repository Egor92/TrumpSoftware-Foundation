using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using TrumpSoftware.Xaml.Mvvm;

namespace TrumpSoftware.WinRT.Mvvm
{
    public class NavigationHost : INavigationHost
    {
        private readonly object _syncRoot = new object();
        private ViewModelBase _currentViewModel;
        private readonly Frame _frame;
        private readonly IDictionary<Type, Type> _pageTypes = new Dictionary<Type, Type>();
        private readonly IDictionary<Type, object> _parameters = new Dictionary<Type, object>();
        private readonly IList<ViewModelBase> _history = new List<ViewModelBase>();
        private int _currentPageIndex = -1;

        public NavigationHost(Frame frame)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");
            _frame = frame;
        }

        public void Register<TPageVM, TPage>(object parameter = null)
            where TPageVM : ViewModelBase
            where TPage : Page
        {
            if (_pageTypes.ContainsKey(typeof(TPageVM)))
                throw new Exception(string.Format("PageViewModel of type {0} has been registered", typeof(TPageVM).FullName));
            _pageTypes.Add(typeof(TPageVM), typeof(TPage));
            if (parameter != null)
                _parameters.Add(typeof(TPageVM), parameter);
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
            Navigate(pageVM, true, false);
        }

        public void RefreshPage()
        {
            if (_currentViewModel != null)
                Navigate(_currentViewModel, false, false);
        }

        private void Navigate<TPageVM>(TPageVM pageVM, bool toRememberInHistory, bool toResetViewModel)
            where TPageVM : ViewModelBase
        {
            var navigatingPageVMType = pageVM.GetType();
            if (!_pageTypes.ContainsKey(navigatingPageVMType))
                throw new Exception(string.Format("PageViewModel of type {0} hasn't been registered", navigatingPageVMType.FullName));
            if (toRememberInHistory)
                RememberInHistory(pageVM);
            if (toResetViewModel)
                ViewModelResetHelper.ResetFields(pageVM);
            var pageType = _pageTypes[navigatingPageVMType];
            var parameter = _parameters.ContainsKey(navigatingPageVMType)
                ? _parameters[navigatingPageVMType]
                : null;
            lock (_syncRoot)
            {
                _currentViewModel = pageVM;
                _frame.Navigated += Frame_Navigated;
                _frame.Navigate(pageType, parameter);
                _frame.Navigated -= Frame_Navigated;
            }
        }

        private void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var frameworkElement = e.Content as FrameworkElement;
            if (frameworkElement != null)
                frameworkElement.DataContext = _currentViewModel;
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
            where TPageVM : ViewModelBase
        {
            for (int i = _history.Count - 1; i > _currentPageIndex; i--)
                _history.RemoveAt(i);
            _history.Add(pageVM);
            _currentPageIndex++;
        }
    }
}
