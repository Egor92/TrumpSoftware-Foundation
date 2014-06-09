namespace TrumpSoftware.Mvvm
{
    public class ModelWrapperViewModel : ViewModelBase
    {
        public object Model { get; private set; }

        public ModelWrapperViewModel(ViewModelBase parent, object model) 
            : base(parent)
        {
            Model = model;
        }
    }
}
