using System;
using System.Collections.Generic;
using TrumpSoftware.Xaml.Navigation;
#if WPF
using System.Windows;
using System.Windows.Controls;
using Dispatcher = System.Windows.Threading.Dispatcher;
#elif WINRT
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Dispatcher = Windows.UI.Core.CoreDispatcher;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Navigation
#elif WINRT
namespace TrumpSoftware.WinRT.Navigation
#endif
{
    public class NavigationManager : INavigationManager
    {
        #region Fields

        private readonly ContentControl _hostControl;
        private readonly IDictionary<Type, NavigationItem> _navigationItemByViewModelType = new Dictionary<Type, NavigationItem>();
        private readonly NavigationHistory _history = new NavigationHistory();

        #endregion

        #region Ctor

        public NavigationManager(ContentControl hostControl)
        {
            if (hostControl == null)
                throw new ArgumentNullException("hostControl");
            _hostControl = hostControl;
        }

        #endregion

        #region Properties

        #region CanGoBack

        public bool CanGoBack
        {
            get { return _history.CanStepBack; }
        }

        #endregion

        #region CanGoForward

        public bool CanGoForward
        {
            get { return _history.CanStepForward; }
        }

        #endregion

        #region CurrentViewModel

        public object CurrentViewModel
        {
            get { return _hostControl.DataContext; }
            private set { _hostControl.DataContext = value; }
        }

        #endregion

        #region CurrentView

        public FrameworkElement CurrentView
        {
            get { return (FrameworkElement) _hostControl.Content; }
            private set { _hostControl.Content = value; }
        }

        #endregion

        #endregion

        #region Events

        #region Navigating

        public event EventHandler<NavigatingEventArgs> Navigating;

        private void RaiseNavigating(NavigatingEventArgs e)
        {
            Navigating?.Invoke(this, e);
        }

        #endregion

        #region Navigated

        public event EventHandler<NavigatedEventArgs> Navigated;

        private void RaiseNavigated(NavigatedEventArgs e)
        {
            Navigated?.Invoke(this, e);
        }

        #endregion

        #endregion

        #region Registration methods

        public void Register<TViewModel>(TViewModel viewModel, FrameworkElement view)
        {
            if (viewModel == null)
                throw new ArgumentNullException("viewModel");
            if (view == null)
                throw new ArgumentNullException("view");

            var viewModelType = typeof (TViewModel);
            if (_navigationItemByViewModelType.ContainsKey(viewModelType))
            {
                var message = string.Format("parameters of type {0} has been registered", viewModelType.FullName);
                throw new Exception(message);
            }
            _navigationItemByViewModelType[viewModelType] = new NavigationItem(viewModel, view);
        }

        public void Register<TViewModel, TView>(TViewModel viewModel)
            where TView : FrameworkElement
        {
            FrameworkElement view = Activator.CreateInstance<TView>();
            Register(viewModel, view);
        }

        #endregion

        #region Navigation methods

        public void Navigate<TViewModel>(object arg)
        {
            Navigate<TViewModel>(arg, true);
        }

        private void Navigate<TViewModel>(object arg, bool toRememberInHistory)
        {
            var modelViewPair = GetNavigationItem<TViewModel>();
            object nextViewModel = modelViewPair.ViewModel;
            FrameworkElement nextView = modelViewPair.View;
            Navigate(nextViewModel, nextView, arg, toRememberInHistory);
        }

        private void Navigate(NavigationItem historyItem)
        {
            var nextViewModel = historyItem.ViewModel;
            var view = historyItem.View;
            Navigate(nextViewModel, view, null, false);
        }

        private void Navigate(object nextViewModel, FrameworkElement nextView, object arg, bool toRememberInHistory)
        {
            var prevViewModel = CurrentViewModel;
            var prevView = CurrentView;

            InvokeInUIThread(() =>
            {
                var navigatingEventArgs = new NavigatingEventArgs(nextViewModel, nextView, arg);
                RaiseNavigating(navigatingEventArgs);
                if (navigatingEventArgs.IsCanceled)
                    return;

                OnNavigating(nextViewModel, arg);
                ChangeView(nextViewModel, nextView, toRememberInHistory);
                OnNavigated(prevViewModel, arg);

                var navigatedEventArgs = new NavigatedEventArgs(prevViewModel, prevView, arg);
                RaiseNavigated(navigatedEventArgs);
            });
        }

        private void OnNavigating(object nextViewModel, object arg)
        {
            var navigationParameters = new NavigationParameters(nextViewModel, arg);
            InvokeOnNavigatingFromIfRequired(CurrentViewModel, navigationParameters);
            InvokeOnNavigatingFromIfRequired(CurrentView, navigationParameters);
        }

        private void ChangeView(object viewModel, FrameworkElement view, bool toRememberInHistory)
        {
            CurrentViewModel = viewModel;
            CurrentView = view;
            if (toRememberInHistory)
            {
                RememberInHistory(viewModel, view);
            }
        }

        private void OnNavigated(object prevViewModel, object arg)
        {
            var navigationParameters = new NavigationParameters(prevViewModel, arg);
            InvokeOnNavigatedToIfRequired(CurrentView, navigationParameters);
            InvokeOnNavigatedToIfRequired(CurrentViewModel, navigationParameters);
        }

        private static void InvokeOnNavigatingFromIfRequired(object obj, NavigationParameters navigationParameters)
        {
            InvokeNavigationCallbackIfRequired(obj, x => x.OnNavigatingFrom(navigationParameters));
        }

        private static void InvokeOnNavigatedToIfRequired(object obj, NavigationParameters navigationParameters)
        {
            InvokeNavigationCallbackIfRequired(obj, x => x.OnNavigatedTo(navigationParameters));
        }

        private static void InvokeNavigationCallbackIfRequired(object obj, Action<INavigationAware> navigationCallback)
        {
            var navigationAware = obj as INavigationAware;
            if (navigationAware != null)
            {
                navigationCallback(navigationAware);
            }
        }

        #endregion

        public void GoBack()
        {
            if (!_history.CanStepBack)
                return;
            var historyItem = _history.StepBack();
            Navigate(historyItem);
        }

        public void GoForward()
        {
            if (!_history.CanStepForward)
                return;
            var historyItem = _history.StepForward();
            Navigate(historyItem);
        }

        private void RememberInHistory(object viewModel, FrameworkElement view)
        {
            _history.Step(viewModel, view);
        }

        private NavigationItem GetNavigationItem<TViewModel>()
        {
            Type viewModelType = typeof (TViewModel);
            if (!_navigationItemByViewModelType.ContainsKey(viewModelType))
            {
                var message = string.Format("parameters of type {0} hasn't been registered", viewModelType.FullName);
                throw new InvalidOperationException(message);
            }
            return _navigationItemByViewModelType[viewModelType];
        }

        private Dispatcher GetUIDispatcher()
        {
            return _hostControl.Dispatcher;
        }

#if WPF
        private void InvokeInUIThread(Action callback)
        {
            var dispatcher = GetUIDispatcher();
            dispatcher?.Invoke(callback);
        }
#elif WINRT
        private void InvokeInUIThread(Action callback)
        {
            var dispatcher = GetUIDispatcher();
            var dispatchedHandler = new DispatchedHandler(callback);
            dispatcher?.RunAsync(CoreDispatcherPriority.Normal, dispatchedHandler);
        }
#endif
    }
}