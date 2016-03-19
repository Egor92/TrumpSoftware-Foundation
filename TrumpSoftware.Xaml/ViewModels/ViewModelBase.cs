using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using TrumpSoftware.Common.Extensions;
using TrumpSoftware.Xaml.Commons;

namespace TrumpSoftware.Xaml.ViewModels
{
    public abstract class ViewModelBase : DisposableReactiveObject
    {
        #region Fields

        private readonly ICollection<IDisposable> _disposables = new List<IDisposable>();
        private readonly ICollection<IEnumerable<IDisposable>> _disposableEnumerables = new List<IEnumerable<IDisposable>>();

        #endregion

        #region Ctor

        protected ViewModelBase(IScheduler scheduler)
        {
            Scheduler = scheduler;
        }

        #endregion

        #region Properties

        #region Scheduler

        protected IScheduler Scheduler { get; private set; }

        #endregion

        #endregion

        #region Overridden methods

        protected override void OnDisposing()
        {
            _disposables.ForEach(x => x?.Dispose());
            _disposableEnumerables.ForEach(x => x?.DisposeEnumerable());
        }

        #endregion

        protected void AddDisposable(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        protected void AddDisposableCollection(IEnumerable<IDisposable> disposables)
        {
            _disposableEnumerables.Add(disposables);
        }

        protected IDisposable SubscribeToPropertyChanged<TSender, TValue>(TSender sender,
                                                                          Expression<Func<TSender, TValue>> propertyFunc,
                                                                          Action<TValue> onChanged)
        {
            return SubscribeToPropertyChanged(sender, propertyFunc, TimeSpan.Zero, onChanged);
        }

        protected IDisposable SubscribeToPropertyChanged<TSender, TValue>(TSender sender,
                                                                          Expression<Func<TSender, TValue>> propertyFunc,
                                                                          TimeSpan delay,
                                                                          Action<TValue> onChanged)
        {
            return sender.ObservableForProperty(propertyFunc)
                         .Throttle(delay, Scheduler)
                         .Select(x => x.GetValue())
                         .Subscribe(onChanged);
        }

        protected IDisposable SubscribeToPropertyChanged<TSender, TValue>(TSender sender,
                                                                          Expression<Func<TSender, TValue>> propertyFunc,
                                                                          Action<TValue> onBeforeChanged,
                                                                          Action<TValue> onAfterChanged)
        {
            return SubscribeToPropertyChanged(sender, propertyFunc, TimeSpan.Zero, onBeforeChanged, onAfterChanged);
        }

        protected IDisposable SubscribeToPropertyChanged<TSender, TValue>(TSender sender,
                                                                          Expression<Func<TSender, TValue>> propertyFunc,
                                                                          TimeSpan delay,
                                                                          Action<TValue> onBeforeChanged,
                                                                          Action<TValue> onAfterChanged)
        {
            var beforeChangedDisposable = sender.ObservableForProperty(propertyFunc, true)
                                                .Throttle(delay, Scheduler)
                                                .Select(x => x.GetValue())
                                                .Subscribe(onBeforeChanged);

            var afterChangedDisposable = sender.ObservableForProperty(propertyFunc)
                                               .Throttle(delay, Scheduler)
                                               .Select(x => x.GetValue())
                                               .Subscribe(onAfterChanged);

            return Disposable.Create(() =>
            {
                beforeChangedDisposable.Dispose();
                afterChangedDisposable.Dispose();
            });
        }
    }
}