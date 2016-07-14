using System;

namespace TrumpSoftware.Xaml.Navigation
{
    public class NavigatingEventArgs
    {
        public object NextViewModel { get; private set; }
        public object NextView { get; private set; }
        public object Arg { get; private set; }
        public bool IsCanceled { get; private set; }

        public NavigatingEventArgs(object nextViewModel, object nextView, object arg)
        {
            if (nextViewModel == null)
                throw new ArgumentNullException("nextViewModel");
            if (nextView == null)
                throw new ArgumentNullException("nextView");
            if (arg == null)
                throw new ArgumentNullException("arg");

            NextViewModel = nextViewModel;
            NextView = nextView;
            Arg = arg;
        }

        public void Cancel()
        {
            IsCanceled = true;
        }
    }
}
