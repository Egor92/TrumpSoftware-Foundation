using System;
using System.Threading.Tasks;

namespace TrumpSoftware.Common
{
    public abstract class TimerBase
    {
        private static readonly object SyncRoot = new object();
        private TimeSpan _time;
        private bool _isRunning;
        private DateTime _intervalStartTime;

        #region TimeFunc

        private Func<TimeSpan, TimeSpan, TimeSpan> _timeFunc;

        private Func<TimeSpan, TimeSpan, TimeSpan> TimeFunc
        {
            get { return _timeFunc ?? (_timeFunc = GetTimeFunc()); }
        }

        protected abstract Func<TimeSpan, TimeSpan, TimeSpan> GetTimeFunc();

        #endregion

        public TimeSpan Time
        {
            get { return _time; }
            set
            {
                lock (SyncRoot)
                {
                    if (_time == value)
                        return;
                    _time = value.Ticks > 0
                        ? value
                        : new TimeSpan();
                    OnTimeChangedInternal();
                }
            }
        }

        public TimeSpan Interval { get; set; }

        #region TimeChanged

        public event EventHandler TimeChanged;

        private void RaiseTimeChanged()
        {
            var handler = TimeChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion

        public TimerBase()
            : this(new TimeSpan(100))
        {
        }

        public TimerBase(TimeSpan interval)
        {
            Interval = interval;
        }

        public void Start()
        {
            if (_isRunning)
                return;
            _isRunning = true;
            StartTimerAsync();
        }

        public void Pause()
        {
            _isRunning = false;
        }

        public void Stop()
        {
            _isRunning = false;
            Time = new TimeSpan();
        }

        protected virtual void OnTimeChanged()
        {

        }

        protected abstract TimeSpan GetDelayInterval();

        private async Task StartTimerAsync()
        {
            _intervalStartTime = DateTime.Now;
            while (true)
            {
                if (!_isRunning)
                    return;
                var delayInterval = GetDelayInterval();
                await Task.Delay(delayInterval);
                if (!_isRunning)
                    return;
                var currentDateTime = DateTime.Now;
                Time = TimeFunc(Time, (currentDateTime - _intervalStartTime));
                _intervalStartTime = currentDateTime;
            }
        }

        private void OnTimeChangedInternal()
        {
            RaiseTimeChanged();
            OnTimeChanged();
        }
    }
}
