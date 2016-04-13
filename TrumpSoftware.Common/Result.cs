using System.Diagnostics;

namespace TrumpSoftware.Common
{
    [DebuggerDisplay("Result: IsSuccess={IsSuccess}, Message={Message}")]
    public class Result
    {
        public bool IsSuccess { get; private set; }

        public string Message { get; private set; }

        public Result()
        {
            IsSuccess = false;
        }

        public Result(bool isSuccess, string message = null)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }

    [DebuggerDisplay("Result: IsSuccess={IsSuccess}, Message={Message}, Data={Data != null ? Data.GetType() : null}")]
    public class Result<T> : Result
    {
        public T Data { get; private set; }
    }
}