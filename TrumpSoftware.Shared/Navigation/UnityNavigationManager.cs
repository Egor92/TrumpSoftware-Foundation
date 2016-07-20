using Microsoft.Practices.Unity;
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
    public class UnityNavigationManager : NavigationManagerBase
    {
        #region Fields

        private readonly IUnityContainer _unityContainer;
        private readonly IDictionary<Type, Type> _viewTypesByViewModelType = new Dictionary<Type, Type>();
        private readonly IDictionary<Type, NavigationItem> _navigationItemsByViewModelType = new Dictionary<Type, NavigationItem>();

        #endregion

        #region Ctor

        public UnityNavigationManager(IUnityContainer unityContainer, ContentControl hostControl)
            : base(hostControl)
        {
            _unityContainer = unityContainer;
        }

        #endregion

        #region Registration methods

        public void Register<TViewModel, TView>()
            where TView : FrameworkElement
        {
            var viewModelType = typeof (TViewModel);
            if (_viewTypesByViewModelType.ContainsKey(viewModelType))
            {
                var message = string.Format("ViewModel of type {0} has been registered", viewModelType.FullName);
                throw new Exception(message);
            }
            _viewTypesByViewModelType[viewModelType] = typeof(TView);
        }

        #endregion

        #region Overridden members

        protected override NavigationItem GetNavigationItem<TViewModel>()
        {
            Type viewModelType = typeof (TViewModel);
            if (!_navigationItemsByViewModelType.ContainsKey(viewModelType))
            {
                if (!_viewTypesByViewModelType.ContainsKey(viewModelType))
                {
                    var message = string.Format("ViewModel of type {0} hasn't been registered", viewModelType.FullName);
                    throw new InvalidOperationException(message);
                }
                var viewType = _viewTypesByViewModelType[viewModelType];
                var viewModel = _unityContainer.Resolve<TViewModel>();
                var view = (FrameworkElement)_unityContainer.Resolve(viewType);
                _navigationItemsByViewModelType[viewModelType] = new NavigationItem(viewModel, view);
            }
            return _navigationItemsByViewModelType[viewModelType];
        }

        #endregion
    }
}