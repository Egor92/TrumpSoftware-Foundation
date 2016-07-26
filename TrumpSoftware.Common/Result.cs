using System.Diagnostics;

namespace TrumpSoftware.Common
{
    [DebuggerDisplay("Result: IsSuccess={IsSuccess}, Message={Message}")]
    public class Result
    {
        public bool IsSuccess { get; private set; }

        public string Message { get; private set; }

        public Result(bool isSuccess, string message = null)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }

    [DebuggerDisplay("Result: IsSuccess={IsSuccess}, Message={Message}, Data={Data != null ? Data.GetType() : null}")]
    public class Result<T> : Result
    {
        public T Data { get; set; }

        public Result(bool isSuccess, string message = null)
            : base(isSuccess, message)
        {
            
        }

        public Result(bool isSuccess, T data)
            : base(isSuccess, null)
        {
            Data = data;
        }

        public static explicit operator Result<T>(Result<object> result)
        {
            var isSuccess = result.IsSuccess;
            var message = result.Message;
            return new Result<T>(isSuccess, message)
            {
                Data = (T)result.Data,
            };
        }

        public static explicit operator Result<object>(Result<T> result)
        {
            var isSuccess = result.IsSuccess;
            var message = result.Message;
            return new Result<object>(isSuccess, message)
            {
                Data = result.Data,
            };
        }
    }
}