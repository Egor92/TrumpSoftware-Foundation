using System;
using System.Windows;
using TrumpSoftware.Xaml.Interfaces;

namespace TrumpSoftware.Wpf.PlatformSpecifics
{
    public class UIThread : IUIThread
    {
        public void Invoke(Action action)
        {
            Application.Current?.Dispatcher?.Invoke(action);
        }
    }
}
