using System;

namespace TrumpSoftware.Xaml.Mvvm
{
    public class NavigatedEventArgs : EventArgs
    {
        public object PageViewModel { get; private set; }
        public object Page { get; private set; }

        public NavigatedEventArgs(object pageVM, object page)
        {
            if (pageVM == null)
                throw new ArgumentNullException("pageVM");
            if (page == null)
                throw new ArgumentNullException("page");

            PageViewModel = pageVM;
            Page = page;
        }
    }
}
