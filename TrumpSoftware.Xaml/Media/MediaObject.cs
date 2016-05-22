using System;

namespace TrumpSoftware.Xaml.Media
{
    public class MediaObject
    {
        #region Properties

        public Uri Uri { get; private set; }

        public double Volume { get; set; } = 0.5;

        public MediaState State { get; private set; } = MediaState.Stop;

        public double Balance { get; set; } = 0;

        #endregion

        #region Ctor

        public MediaObject(Uri uri)
        {
            Uri = uri;
        }

        #endregion

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
    }
}
