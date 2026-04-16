namespace Magnus.API.Helpers;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    public string? TraceId { get; set; }

    public static ApiResponse<T> SuccessResponse(T? data, string message) =>
        new()
        {
            Success = true,
            Message = message,
            Data = data
        };

    public static ApiResponse<T> FailureResponse(string message, IEnumerable<string>? errors = null, string? traceId = null) =>
        new()
        {
            Success = false,
            Message = message,
            Errors = errors,
            TraceId = traceId
        };
}
