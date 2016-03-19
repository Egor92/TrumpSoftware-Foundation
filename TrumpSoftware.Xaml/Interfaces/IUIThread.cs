using System;

namespace TrumpSoftware.Xaml.Interfaces
{
    public interface IUIThread
    {
        void Invoke(Action action);
    }
}
