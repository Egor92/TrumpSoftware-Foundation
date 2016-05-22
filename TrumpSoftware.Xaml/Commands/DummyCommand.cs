using System;
using System.Windows.Input;

namespace TrumpSoftware.Xaml.Commands
{
    public class DummyCommand : ICommand
    {
        #region Fields

        private readonly bool _canExecute;

        #endregion

        #region Ctor

        private DummyCommand(bool canExecute)
        {
            _canExecute = canExecute;
        }

        #endregion

        #region Properties

        public static ICommand ExecutableCommand { get; } = new DummyCommand(true);

        public static ICommand NotExecutableCommand { get; } = new DummyCommand(false);

        #endregion

        #region Implementation of ICommand

        public void Execute(object parameter)
        {
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}
