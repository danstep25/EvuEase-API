namespace EvuEase.Api.Common;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public ErrorResponse? Error { get; set; }
    public bool Success => Error == null;

    public ApiResponse(T? data)
    {
        Data = data;
        Error = null;
    }

    public ApiResponse(ErrorResponse error)
    {
        Data = default;
        Error = error;
    }

    public static ApiResponse<T> SuccessResponse(T data)
    {
        return new ApiResponse<T>(data);
    }

    public static ApiResponse<T> ErrorResponse(string message, int? statusCode = null, object? details = null)
    {
        return new ApiResponse<T>(new ErrorResponse
        {
            Message = message,
            StatusCode = statusCode,
            Details = details
        });
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public int? StatusCode { get; set; }
    public object? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

