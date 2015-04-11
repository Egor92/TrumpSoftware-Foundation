namespace TrumpSoftware.Xaml.Mvvm
{
    public class WindowViewModel : NotificationObject
    {
        #region IsOpened

        private bool _isOpened;

        public bool IsOpened
        {
            get { return _isOpened; }
            set { SetProperty(ref _isOpened, value, OnIsOpenedChanged); }
        }

        protected virtual void OnIsOpenedChanged()
        {
            if (IsOpened)
                OnOpen();
            else
                OnClose();
        }

        protected virtual void OnOpen()
        {

        }

        #endregion

        #region CloseCommand

        private DelegateCommand _closeCommand;

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new DelegateCommand(Close, CanClose)); }
        }

        public void Close()
        {
            IsOpened = false;
        }

        protected virtual void OnClose()
        {
            
        }

        protected virtual bool CanClose()
        {
            return true;
        }

        #endregion

        #region SaveCommand

        private DelegateCommand _saveCommand;

        public DelegateCommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new DelegateCommand(OnSave, CanSave)); }
        }

        protected virtual void OnSave()
        {
        }

        protected virtual bool CanSave()
        {
            return true;
        }

        #endregion

        public void Open()
        {
            IsOpened = true;
        }
    }
}
