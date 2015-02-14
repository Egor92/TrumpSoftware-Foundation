using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrumpSoftware.Xaml.Mvvm
{
    public class NavigatedEventArgs : EventArgs
    {
        public PageViewModel PageViewModel { get; private set; }
        public object Page { get; private set; }

        public NavigatedEventArgs(PageViewModel pageVM, object page)
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
