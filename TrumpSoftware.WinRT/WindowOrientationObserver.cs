using System;
using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TrumpSoftware.WinRT
{
    public class WindowOrientationObserver
    {
        private readonly IList<Control> _subscribers = new List<Control>();
        private ApplicationViewOrientation _currentOrientation;

        #region Instance

        private static WindowOrientationObserver _instance;

        private static WindowOrientationObserver Instance
        {
            get { return _instance ?? (_instance = new WindowOrientationObserver()); }
        }

        #endregion

        private WindowOrientationObserver()
        {
            Window.Current.SizeChanged += Window_SizeChanged;
            _currentOrientation = ApplicationView.GetForCurrentView().Orientation;
        }

        public static void AddSubscriber(Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");
            if (!Instance._subscribers.Contains(control))
                Instance._subscribers.Add(control);
            SetOrientation(control);
        }

        public static void RemoveSubscriber(Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");
            Instance._subscribers.Remove(control);
        }

        private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            var newOrientation = ApplicationView.GetForCurrentView().Orientation;
            if (Instance._currentOrientation == newOrientation)
                return;
            Instance._currentOrientation = newOrientation;
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
