using System;
using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TrumpSoftware.WinRT
{
    internal class WindowOrientationObserver
    {
        private readonly IList<Control> _subscribers = new List<Control>();

        #region Instance

        private static WindowOrientationObserver _instance;

        private static WindowOrientationObserver Instance
        {
            get { return _instance ?? (_instance = new WindowOrientationObserver()); }
        }

        #endregion

        private ApplicationViewOrientation _currentOrientation;

        private WindowOrientationObserver()
        {
            Window.Current.SizeChanged += Window_SizeChanged;
            _currentOrientation = ApplicationView.GetForCurrentView().Orientation;
        }

        internal static void AddSubscriber(Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");
            if (!Instance._subscribers.Contains(control))
                Instance._subscribers.Add(control);
            SetOrientation(control);
        }

        internal static void RemoveSubscriber(Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");
            Instance._subscribers.Remove(control);
        }

        private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            var newOrientation = ApplicationView.GetForCurrentView().Orientation;
            if (_currentOrientation == newOrientation)
                return;
            _currentOrientation = newOrientation;
            foreach (var subscriber in _subscribers)
                SetOrientation(subscriber);
        }

        private static void SetOrientation(Control control)
        {
            var stateName = Instance._currentOrientation.ToString();
            VisualStateManager.GoToState(control, stateName, false);
        }
    }
}
