using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace TrumpSoftware.Xaml.ViewModels
{
    public abstract class BusyActiveAwareViewModel : ActiveAwareViewModel
    {
        #region Fields

        private readonly ManualResetEvent _activatingManualEvent = new ManualResetEvent(true);

        #endregion

        #region Ctor

        protected BusyActiveAwareViewModel(IScheduler scheduler)
            : base(scheduler)
        {
        }

        #endregion

        #region Overridden members

        protected override void InvokeIsActiveChangedAction(Action action)
        {
            _activatingManualEvent.WaitOne();
            Task.Run(() =>
            {
                action();
            }).ContinueWith(_ =>
            {
                _activatingManualEvent.Set();
            });
        }

        #endregion
    }
}
