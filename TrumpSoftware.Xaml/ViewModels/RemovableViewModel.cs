using System;
using System.Reactive.Concurrency;
using System.Windows.Input;
using ReactiveUI;
using TrumpSoftware.Common.Interfaces;

namespace TrumpSoftware.Xaml.ViewModels
{
    public abstract class RemovableViewModel : ViewModelBase, IRemovable
    {
        #region Ctor

        protected RemovableViewModel(IScheduler scheduler)
            : base(scheduler)
        {
        }

        #endregion

        #region RemoveCommand

        private ICommand _removeCommand;

        public ICommand RemoveCommand
        {
            get { return _removeCommand ?? (_removeCommand = GetRemoveCommand()); }
        }

        private ReactiveCommand<object> GetRemoveCommand()
        {
            var command = ReactiveCommand.Create(null, Scheduler);
            command.Subscribe(OnRemove);
            AddDisposable(command);
            return command;
        }

        private void OnRemove(object o)
        {
            RaiseRemoving();
        }

        #endregion

        #region Removing

        public event EventHandler Removing;

        private void RaiseRemoving()
        {
            Removing?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
