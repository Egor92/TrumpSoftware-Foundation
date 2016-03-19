using System;
using System.Reactive.Concurrency;

namespace TrumpSoftware.Xaml.ViewModels
{
    public interface ISettingsItemViewModel<out T>
    {
        T SettingsSource { get; }
    }

    public abstract class SettingsItemViewModel<T> : RemovableViewModel, ISettingsItemViewModel<T>
    {
        #region Ctor

        protected SettingsItemViewModel(IScheduler scheduler, T settingsSource)
            : base(scheduler)
        {
            if (settingsSource == null)
                throw new ArgumentNullException("settingsSource");
            SettingsSource = settingsSource;
        }

        #endregion

        #region Properties

        #region SettingsSource

        public T SettingsSource { get; private set; }

        #endregion

        #endregion

        public abstract void Save();
    }
}
