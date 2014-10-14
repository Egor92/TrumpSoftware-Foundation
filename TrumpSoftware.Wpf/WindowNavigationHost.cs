using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Navigation;
using TrumpSoftware.Mvvm;

namespace TrumpSoftware.Wpf
{
    public class WindowNavigationHost : INavigationHost
    {
        private readonly NavigationWindow _navigationWindow;
        private readonly IDictionary<Type, Page> _pages = new Dictionary<Type, Page>();

        public WindowNavigationHost(NavigationWindow navigationWindow)
        {
            if (navigationWindow == null)
                throw new ArgumentNullException("navigationWindow");
            _navigationWindow = navigationWindow;
        }

        public void Register<TPageVM>(Page page)
            where TPageVM : ViewModelBase
        {
            if (_pages.ContainsKey(typeof(TPageVM)))
                throw new Exception(string.Format("PageViewModel of type {0} has been registered", typeof(TPageVM).FullName));
            _pages.Add(typeof (TPageVM), page);
        }

        public void Navigate<TPageVM>(TPageVM pageVM) 
            where TPageVM : ViewModelBase
        {
            if (!_pages.ContainsKey(typeof(TPageVM)))
                throw new Exception(string.Format("PageViewModel of type {0} hasn't been registered", typeof(TPageVM).FullName));
            var page = _pages[typeof(TPageVM)];
            page.DataContext = pageVM;
            _navigationWindow.Navigate(page, pageVM);
        }
    }
}
