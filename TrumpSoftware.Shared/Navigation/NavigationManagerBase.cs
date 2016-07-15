using System;
using TrumpSoftware.Xaml.Navigation;
#if WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
#elif WINRT
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Navigation
#elif WINRT
namespace TrumpSoftware.WinRT.Navigation
#endif
{
    public abstract class NavigationManagerBase : INavigationManager
    {
        #region Fields

        private readonly ContentControl _hostControl;
        private readonly NavigationHistory _history = new NavigationHistory();

        #endregion

        #region Ctor

        protected NavigationManagerBase(ContentControl hostControl)
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

        private void Navigate(NavigationItem historyItem, NavigationDirection direction)
        {
            var nextViewModel = historyItem.ViewModel;
            var view = historyItem.View;
            Navigate(nextViewModel, view, direction, false);
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
            Navigate(historyItem, NavigationDirection.Back);
        }

        public void GoForward()
        {
            if (!_history.CanStepForward)
                return;
            var historyItem = _history.StepForward();
            Navigate(historyItem, NavigationDirection.Forward);
        }

        private void RememberInHistory(object viewModel, FrameworkElement view)
        {
            _history.Step(viewModel, view);
        }

        protected abstract NavigationItem GetNavigationItem<TViewModel>();

#if WPF
        private void InvokeInUIThread(Action callback)
        {
            var dispatcher = GetUIDispatcher();
            dispatcher?.Invoke(callback);
        }

        private Dispatcher GetUIDispatcher()
        {
            return _hostControl.Dispatcher;
        }
#elif WINRT
        private void InvokeInUIThread(Action callback)
        {
            var dispatcher = GetUIDispatcher();
            var dispatchedHandler = new DispatchedHandler(callback);
            dispatcher?.RunAsync(CoreDispatcherPriority.Normal, dispatchedHandler);
        }

        private CoreDispatcher GetUIDispatcher()
        {
            return _hostControl.Dispatcher;
        }
#endif
    }
}