using EvuEase.Domain.Enums;

namespace EvuEase.Application.Common;

public class HandlerResult<T>
{
    public T? Data { get; private set; }
    public ErrorResponse? Error { get; private set; }
    public bool IsSuccess => Error == null;
    public bool IsFailure => Error != null;

    private HandlerResult(T? data, ErrorResponse? error)
    {
        Data = data;
        Error = error;
    }

    public static HandlerResult<T> Success(T data)
    {
        return new HandlerResult<T>(data, null);
    }

    public static HandlerResult<T> Failure(string message, StatusCode statusCode)
    {
        return new HandlerResult<T>(default, new ErrorResponse(message, statusCode));
    }

    public static HandlerResult<T> Failure(ErrorResponse error)
    {
        return new HandlerResult<T>(default, error);
    }

    public class ErrorResponse
    {
        public StatusCode StatusCode { get; set; }
        public string Message { get; set; }

        public ErrorResponse(string message, StatusCode statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }
    }
}
