using System;

namespace TrumpSoftware.Common.Exceptions
{
    public class UnhandledCaseException : Exception
    {
        private readonly string _message;

        public override string Message
        {
            get { return _message; }
        }

        public UnhandledCaseException(Type enumType, object value)
        {
            _message = string.Format("Unhandled case of {0}.{1}", enumType, value);
        }

        public UnhandledCaseException(object value)
        {
            _message = string.Format("Unhandled case of {0}", value);
        }

        public UnhandledCaseException(string message)
        {
            _message = message;
        }

        public UnhandledCaseException()
        {
        }
    }
}
