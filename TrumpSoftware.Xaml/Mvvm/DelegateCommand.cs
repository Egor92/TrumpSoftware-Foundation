using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TrumpSoftware.Xaml.Mvvm
{
	public class DelegateCommand : ICommand
	{
		private readonly Action _command;
		private readonly Func<bool> _canExecute;

	    public event EventHandler CanExecuteChanged;

		public DelegateCommand(Action command, Func<bool> canExecute = null)
		{
            if (command == null)
                throw new ArgumentNullException("command");
            _canExecute = canExecute;
            _command = command;
		}

	    public static DelegateCommand GetAsyncCommand(Func<Task> command, Func<bool> canExecute = null)
	    {
            return new DelegateCommand(() => command(), canExecute);
	    }

		public void Execute(object parameter)
		{
			_command();
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

    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _command;
        private readonly Func<T, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> command, Func<T, bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            _canExecute = canExecute;
            _command = command;
        }

        public static DelegateCommand<T> GetAsyncCommand(Func<T, Task> command, Func<T, bool> canExecute = null)
        {
            return new DelegateCommand<T>(x => command(x), canExecute);
        }

        public void Execute(object parameter)
        {
            if (!(parameter is T))
                throw new ArgumentException(string.Format("parameter is not assignable to type {0}", typeof(T).FullName), "parameter");
            _command((T)parameter);
        }

        public bool CanExecute(object parameter)
        {
            if (!(parameter is T))
                throw new ArgumentException(string.Format("parameter is not assignable to type {0}", typeof(T).FullName), "parameter");
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}