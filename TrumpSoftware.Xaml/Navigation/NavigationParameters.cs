using System;

namespace TrumpSoftware.Xaml.Navigation
{
    public class NavigationParameters
    {
        public NavigationParameters(object viewModel, object arg)
        {
            if (viewModel == null)
                throw new ArgumentNullException("viewModel");
            ViewModel = viewModel;
            Arg = arg;
        }

        public object ViewModel { get; private set; }
        public object Arg { get; private set; }
    }
}
