using System;
using TrumpSoftware.Mvvm;

namespace TrumpSoftware.Wpf
{
    public abstract class PageViewModel : ViewModelBase
    {
        protected INavigationHost NavigationHost { get; private set; }

        protected PageViewModel(ViewModelBase parent, INavigationHost navigationHost)
            : base(parent)
        {
            if (navigationHost == null)
                throw new ArgumentNullException("navigationHost");
            NavigationHost = navigationHost;
        }
    }
}
