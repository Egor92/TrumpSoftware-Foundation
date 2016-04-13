using System;

namespace TrumpSoftware.Common.Exceptions
{
    public class UnhandledCaseException : Exception
    {
        #region Properties

        #region Message

        public override string Message { get; }

        #endregion

        #endregion

        #region Ctor

        public UnhandledCaseException(Type enumType, object value)
        {
            Message = string.Format("Unhandled case of {0}.{1}", enumType, value);
        }

        public UnhandledCaseException(object value)
        {
            if (value == null)
            {
                Message = string.Format("Unhandled case of 'null'");
            }
            else
            {
                Message = string.Format("Unhandled case of {0}.{1}", value.GetType(), value);
            }
        }

        public UnhandledCaseException(string message)
        {
            Message = message;
        }

        public UnhandledCaseException()
        {
        }

        #endregion
    }
}
