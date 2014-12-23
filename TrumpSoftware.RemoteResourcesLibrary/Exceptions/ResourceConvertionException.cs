using System;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    public class ResourceConvertionException : Exception
    {
        private readonly string _message;

        public override string Message
        {
            get { return _message; }
        }

        internal ResourceConvertionException(string message)
        {
            _message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}