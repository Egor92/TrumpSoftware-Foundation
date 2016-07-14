using System;

namespace TrumpSoftware.Xaml.Navigation
{
    public class NavigatedEventArgs : EventArgs
    {
        public object PrevViewModel { get; private set; }
        public object PrevView { get; private set; }
        public object Arg { get; private set; }

        public NavigatedEventArgs(object prevViewModel, object prevView, object arg)
        {
            if (prevViewModel == null)
                throw new ArgumentNullException("prevViewModel");
            if (prevView == null)
                throw new ArgumentNullException("prevView");
            if (arg == null)
                throw new ArgumentNullException("arg");

            PrevViewModel = prevViewModel;
            PrevView = prevView;
            Arg = arg;
        }
    }
}
