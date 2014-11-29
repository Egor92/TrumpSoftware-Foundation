using System;

namespace TrumpSoftware.Xaml.Mvvm
{
    public abstract class DataItemViewModel : ViewModelBase
    {
        public object Model { get; private set; }

        protected DataItemViewModel(object model, ViewModelBase parent = null)
            : base(parent)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            Model = model;
        }
    }
}
