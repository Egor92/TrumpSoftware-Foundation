using System;
using ReactiveUI;

namespace TrumpSoftware.Xaml.Commons
{
    public abstract class DisposableReactiveObject : ReactiveObject, IDisposable
    {
        #region Implementation of IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                OnDisposing();
            }

            _disposed = true;
        }

        protected virtual void OnDisposing()
        {

        }

        ~DisposableReactiveObject()
        {
            Dispose(false);
        }

        #endregion
    }
}