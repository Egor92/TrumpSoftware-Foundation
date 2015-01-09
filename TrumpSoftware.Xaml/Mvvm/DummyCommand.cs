using System;
using System.Windows.Input;

namespace TrumpSoftware.Xaml.Mvvm
{
    public class DummyCommand : ICommand
    {
        private readonly bool _canExecute;

        public event EventHandler CanExecuteChanged;

        public static readonly ICommand ExecutableCommand = new DummyCommand(true);
        public static readonly ICommand NotExecutableCommand = new DummyCommand(false);

        private DummyCommand(bool canExecute)
        {
            _canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }
    }
}
