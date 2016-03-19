using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Prism;
using ReactiveUI;

namespace TrumpSoftware.Xaml.ViewModels
{
    public abstract class ActiveAwareViewModel : ViewModelBase, IActiveAware
    {
        #region Ctor

        protected ActiveAwareViewModel(IScheduler scheduler)
            : base(scheduler)
        {
            SubscribeToPropertyChanges();
        }

        private void SubscribeToPropertyChanges()
        {
            AddDisposableCollection(new[]
            {
                SubscribeToIsActiveIsSetTrue(),
                SubscribeToIsActiveIsSetFalse(),
            });
        }

        #endregion

        #region Properties

        #region ActivationDelay

        protected virtual TimeSpan ActivationDelay
        {
            get { return TimeSpan.FromMilliseconds(0); }
        }

        #endregion

        #region DeactivationDelay

        protected virtual TimeSpan DeactivationDelay
        {
            get { return TimeSpan.FromMilliseconds(0); }
        }

        #endregion

        #endregion

        #region Implemetation of IActiveAware

        #region IsActive

        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            set { this.RaiseAndSetIfChanged(ref _isActive, value); }
        }

        private IDisposable SubscribeToIsActiveIsSetTrue()
        {
            return this.ObservableForProperty(x => x.IsActive)
                .Throttle(ActivationDelay, Scheduler)
                .Select(x => x.GetValue())
                .Subscribe(OnIsActiveIsSetTrue);
        }

        private void OnIsActiveIsSetTrue(bool isActive)
        {
            OnIsActiveChanged(isActive, OnActivate);
        }

        private IDisposable SubscribeToIsActiveIsSetFalse()
        {
            return this.ObservableForProperty(x => x.IsActive)
                .Throttle(DeactivationDelay, Scheduler)
                .Select(x => x.GetValue())
                .Subscribe(OnIsActiveIsSetFalse);
        }

        private void OnIsActiveIsSetFalse(bool isActive)
        {
            OnIsActiveChanged(!isActive, OnDeactivate);
        }

        private void OnIsActiveChanged(bool condition, Action action)
        {
            if (!condition)
                return;
            RaiseIsActiveChanged();
            InvokeIsActiveChangedAction(action);
        }

        protected virtual void InvokeIsActiveChangedAction(Action action)
        {
            action();
        }

        #endregion

        #region IsActiveChanged

        public event EventHandler IsActiveChanged;

        private void RaiseIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        protected virtual void OnActivate()
        {

        }

        protected virtual void OnDeactivate()
        {

        }
    }
}
