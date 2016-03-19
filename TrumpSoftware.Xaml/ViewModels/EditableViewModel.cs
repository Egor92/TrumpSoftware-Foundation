using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using ReactiveUI;

namespace TrumpSoftware.Xaml.ViewModels
{
    public abstract class EditableViewModel : ViewModelBase
    {
        #region Fields

        private static readonly IEnumerable<string> IgnoringPropertyNames = new[]
        {
            "IsDirty"
        };

        #endregion

        #region Ctor

        protected EditableViewModel(IScheduler scheduler)
            : base(scheduler)
        {
            Initialize();
        }

        private void Initialize()
        {
            PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Properties

        #region IsDirty

        private bool _isDirty;

        public bool IsDirty
        {
            get { return _isDirty; }
            private set { this.RaiseAndSetIfChanged(ref _isDirty, value); }
        }

        #endregion

        #endregion

        #region Overridden methods

        protected override void OnDisposing()
        {
            base.OnDisposing();
            PropertyChanged -= OnPropertyChanged;
        }

        #endregion

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!IsDirty && IgnoringPropertyNames.Contains(e.PropertyName))
                IsDirty = true;
        }
    }
}
