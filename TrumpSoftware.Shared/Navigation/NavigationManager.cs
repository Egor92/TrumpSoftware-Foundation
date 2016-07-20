using System;
using System.Collections.Generic;
#if WPF
using System.Windows;
using System.Windows.Controls;
#elif WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Navigation
#elif WINRT
namespace TrumpSoftware.WinRT.Navigation
#endif
{
    public class NavigationManager : NavigationManagerBase
    {
        #region Fields

        private readonly IDictionary<Type, NavigationItem> _navigationItemsByViewModelType = new Dictionary<Type, NavigationItem>();

        #endregion

        #region Ctor

        public NavigationManager(ContentControl hostControl)
            : base(hostControl)
        {
        }

        #endregion

        #region Registration methods

        public void Register<TViewModel>(TViewModel viewModel, FrameworkElement view)
        {
            if (viewModel == null)
                throw new ArgumentNullException("viewModel");
            if (view == null)
                throw new ArgumentNullException("view");

            var viewModelType = typeof (TViewModel);
            if (_navigationItemsByViewModelType.ContainsKey(viewModelType))
            {
                var message = string.Format("ViewModel of type {0} has been registered", viewModelType.FullName);
                throw new Exception(message);
            }
            _navigationItemsByViewModelType[viewModelType] = new NavigationItem(viewModel, view);
        }

        public void Register<TViewModel, TView>(TViewModel viewModel)
            where TView : FrameworkElement
        {
            FrameworkElement view = Activator.CreateInstance<TView>();
            Register(viewModel, view);
        }

        #endregion

        #region Overridden members

        protected override NavigationItem GetNavigationItem<TViewModel>()
        {
            Type viewModelType = typeof (TViewModel);
            if (!_navigationItemsByViewModelType.ContainsKey(viewModelType))
            {
                var message = string.Format("ViewModel of type {0} hasn't been registered", viewModelType.FullName);
                throw new InvalidOperationException(message);
            }
            return _navigationItemsByViewModelType[viewModelType];
        }

        #endregion
    }
}