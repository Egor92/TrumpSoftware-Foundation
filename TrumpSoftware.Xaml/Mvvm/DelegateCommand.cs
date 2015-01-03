using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TrumpSoftware.Xaml.Mvvm
{
	public class DelegateCommand : ICommand
	{
		private readonly Func<Task> _command;
		private readonly Func<bool> _canExecute;

	    public event EventHandler CanExecuteChanged;

		public DelegateCommand(Action command, Func<bool> canExecute = null)
            : this (async () => command(), canExecute)
		{
		}

        private DelegateCommand(Func<Task> command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            _canExecute = canExecute;
            _command = command;
        }

	    public static DelegateCommand GetAsyncCommand(Func<Task> command, Func<bool> canExecute = null)
	    {
            return new DelegateCommand(command, canExecute);
	    }

		public async void Execute(object parameter)
		{
			await _command();
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute();
		}

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
	}
}