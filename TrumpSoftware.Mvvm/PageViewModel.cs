using System;

namespace TrumpSoftware.Mvvm
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
