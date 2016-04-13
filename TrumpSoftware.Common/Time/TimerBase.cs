using System;
using System.Threading.Tasks;

namespace TrumpSoftware.Common.Time
{
    public abstract class TimerBase
    {
        #region Fields

        private static readonly object SyncRoot = new object();
        private TimeSpan _time;
        private bool _isRunning;
        private DateTime _intervalStartTime;

        #endregion

        #region Properties

        #region Time

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

        private void OnTimeChangedInternal()
        {
            OnTimeChanged();
            RaiseTimeChanged();
        }

        protected virtual void OnTimeChanged()
        {

        }

        #endregion

        #region Interval

        public TimeSpan Interval { get; set; }

        #endregion

        #endregion

        #region Events

        #region TimeChanged

        public event EventHandler TimeChanged;

        private void RaiseTimeChanged()
        {
            var handler = TimeChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion

        #endregion

        #region Ctor

        public TimerBase()
            : this(new TimeSpan(100))
        {
        }

        public TimerBase(TimeSpan interval)
        {
            Interval = interval;
        }

        #endregion

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

        protected abstract TimeSpan GetDelayInterval();

        protected abstract TimeSpan AggregateTime(TimeSpan left, TimeSpan right);

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
                Time = AggregateTime(Time, (currentDateTime - _intervalStartTime));
                _intervalStartTime = currentDateTime;
            }
        }
    }
}
