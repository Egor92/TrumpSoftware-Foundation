using System;
using System.Threading.Tasks;

namespace TrumpSoftware.Common
{
    public sealed class Timer
    {
        private static readonly object SyncRoot = new object();
        private TimeSpan _time;
        private bool _isRunning;
        private DateTime _intervalStartTime;

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
                    OnTimeChanged();
                }
            }
        }

        public TimeSpan Interval { get; set; }

        public event EventHandler TimeChanged;

        public event EventHandler TimeHasExpired;

        public Timer()
            : this(new TimeSpan(100))
        {
        }

        public Timer(TimeSpan interval)
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

        public void Start(TimeSpan newTime)
        {
            Time = newTime;
            Start();
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

        private async Task StartTimerAsync()
        {
            _intervalStartTime = DateTime.Now;
            while (true)
            {
                if (!_isRunning)
                    return;
                var delayInterval = Interval < Time
                    ? Interval
                    : Time;
                await Task.Delay(delayInterval);
                if (!_isRunning)
                    return;
                var currentDateTime = DateTime.Now;
                Time -= (currentDateTime - _intervalStartTime);
                _intervalStartTime = currentDateTime;
            }
        }

        private void OnTimeChanged()
        {
            RaiseTimeChanged();
            if (Time.Ticks <= 0)
                RaiseTimeHasExpired();
        }

        private void RaiseTimeChanged()
        {
            var handler = TimeChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void RaiseTimeHasExpired()
        {
            var handler = TimeHasExpired;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
