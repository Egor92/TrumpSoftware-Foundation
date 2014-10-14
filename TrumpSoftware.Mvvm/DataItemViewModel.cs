using System;

namespace TrumpSoftware.Mvvm
{
    public abstract class DataItemViewModel : ViewModelBase
    {
        public IDataItem Model { get; private set; }

        public int Id
        {
            get { return Model.Id; }
            set
            {
                if (Model.Id == value)
                    return;
                Model.Id = value;
                RaisePropertyChanged(() => Id);
            }
        }

        protected DataItemViewModel(ViewModelBase parent, IDataItem model)
            : base(parent)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            Model = model;
        }
    }
}
