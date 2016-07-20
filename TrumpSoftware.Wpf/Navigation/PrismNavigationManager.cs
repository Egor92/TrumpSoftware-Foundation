using Prism.Regions;
using System.Linq;
using System;
using System.Collections.Generic;
using TrumpSoftware.Xaml.Navigation;
using NavigationParameters = Prism.Regions.NavigationParameters;
using System.Windows;
using System.Windows.Controls;

namespace TrumpSoftware.Wpf.Navigation
{
    public class PrismNavigationManager : INavigationManager
    {
        #region Fields

        private readonly IRegionManager _regionManager;

        public const string ArgKey = @"arg";
        private readonly string _regionName;
        private readonly NavigationHistory _navigationHistory = new NavigationHistory();
        private readonly IDictionary<Type, string> _targetsByViewModelType = new Dictionary<Type, string>();

        #endregion

        #region Ctor

        public PrismNavigationManager(IRegionManager regionManager, ContentControl hostControl)
        {
            _regionManager = regionManager;
            _regionName = string.Format("NavigationRegion_{0}", Guid.NewGuid());

            RegionManager.SetRegionName(hostControl, _regionName);
        }

        #endregion

        #region Properties

        #region CanGoBack

        public bool CanGoBack
        {
            get { return _navigationHistory.CanStepBack; }
        }

        #endregion

        #region CanGoForward

        public bool CanGoForward
        {
            get { return _navigationHistory.CanStepForward; }
        }

        #endregion

        #endregion

        #region Registration methods

        public void Register<TViewModel>(string target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            var viewModelType = typeof(TViewModel);
            if (_targetsByViewModelType.ContainsKey(viewModelType))
            {
                var message = string.Format("ViewModel of type {0} has been registered", viewModelType.FullName);
                throw new Exception(message);
            }
            _targetsByViewModelType[viewModelType] = target;
        }

        #endregion

        #region Navigation methods

        public void Navigate<TViewModel>(object arg)
        {
            var viewModelType = typeof(TViewModel);
            if (!_targetsByViewModelType.ContainsKey(viewModelType))
            {
                var message = string.Format("ViewModel of type {0} hasn't been registered", viewModelType.FullName);
                throw new InvalidOperationException(message);
            }
            var target = _targetsByViewModelType[viewModelType];
            Navigate(target, arg);
        }

        private void Navigate(string target, object arg)
        {
            var navigationParameters = new NavigationParameters()
            {
                { ArgKey, arg }
            };
            var region = _regionManager.Regions[_regionName];
            region.RequestNavigate(target, navigationResult =>
            {
                if (navigationResult.Result != true)
                    return;
                var view = region.ActiveViews.FirstOrDefault() as FrameworkElement;
                if (view == null)
                    return;
                var viewModel = view.DataContext;
                _navigationHistory.Step(viewModel, view);
            }, navigationParameters);
        }

        private void Navigate(NavigationItem navigationItem)
        {
            var view = navigationItem.View;
            _regionManager.Regions[_regionName].Activate(view);
        }

        #endregion

        public void GoBack()
        {
            var navigationItem = _navigationHistory.StepBack();
            Navigate(navigationItem);
        }

        public void GoForward()
        {
            var navigationItem = _navigationHistory.StepForward();
            Navigate(navigationItem);
        }
    }
}
