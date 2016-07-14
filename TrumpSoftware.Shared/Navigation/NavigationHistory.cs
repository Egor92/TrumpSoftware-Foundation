using System.Collections.Generic;
#if WPF
using System.Windows;
#elif WINRT
using Windows.UI.Xaml;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Navigation
#elif WINRT
namespace TrumpSoftware.WinRT.Navigation
#endif
{
    internal class NavigationHistory
    {
        #region Fields

        private int _currentIndex = -1;
        private readonly IList<NavigationItem> _historyItems = new List<NavigationItem>();

        #endregion

        #region Properties

        #region CanStepBack

        public bool CanStepBack
        {
            get { return _currentIndex > 0; }
        }

        #endregion

        #region CanStepForward

        public bool CanStepForward
        {
            get { return _currentIndex < _historyItems.Count - 1; }
        }

        #endregion

        #endregion

        public void Step(object viewModel, FrameworkElement view)
        {
            var nextItemIndex = _currentIndex + 1;
            while (nextItemIndex < _historyItems.Count)
            {
                _historyItems.RemoveAt(nextItemIndex);
            }
            var historyItem = new NavigationItem(viewModel, view);
            _historyItems.Add(historyItem);
            _currentIndex++;
        }

        public NavigationItem StepBack()
        {
            if (!CanStepBack)
                return null;
            _currentIndex--;
            return _historyItems[_currentIndex];
        }

        public NavigationItem StepForward()
        {
            if (!CanStepForward)
                return null;
            _currentIndex++;
            return _historyItems[_currentIndex];
        }
    }
}