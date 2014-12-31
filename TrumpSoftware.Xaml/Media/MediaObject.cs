using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TrumpSoftware.Common;

namespace TrumpSoftware.Xaml.Media
{
    public class MediaObject : INotifyPropertyChanged
    {
        private MediaState _state = MediaState.Stop;
        private double _volume = 0.5;
        private double _balance = 0;

        public Uri Uri { get; private set; }

        public double Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        public MediaState State
        {
            get { return _state; }
            private set
            {
                if (_state == value)
                    return;
                var oldState = _state;
                _state = value;
                OnPropertyChanged();
                RaiseStateChanged(_state, oldState);
            }
        }

        public double Balance
        {
            get { return _balance; }
            set { _balance = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangedEventHandler<MediaState> StateChanged;

        public MediaObject(Uri uri)
        {
            Uri = uri;
        }

        public void Play()
        {
            State = MediaState.Play;
        }

        public void Pause()
        {
            State = MediaState.Pause;
        }

        public void Stop()
        {
            State = MediaState.Stop;
        }

        private void RaiseStateChanged(MediaState newState, MediaState oldState)
        {
            var handler = StateChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs<MediaState>(newState, oldState));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
