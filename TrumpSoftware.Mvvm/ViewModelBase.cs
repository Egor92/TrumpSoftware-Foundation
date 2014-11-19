namespace TrumpSoftware.Mvvm
{
    public abstract class ViewModelBase : NotificationObject
    {
        public ViewModelBase Parent { get; protected internal set; }

        protected ViewModelBase(ViewModelBase parent = null)
        {
            Parent = parent;
        }

        protected TViewModel GetAncestor<TViewModel>()
            where TViewModel : class
        {
            if (Parent == null)
                return null;
            if (Parent is TViewModel)
                return Parent as TViewModel;
            return Parent.GetAncestor<TViewModel>();
        }
    }
}
