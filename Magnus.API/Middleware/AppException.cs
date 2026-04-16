using System.Net;

namespace Magnus.API.Middleware;

public class AppException : Exception
{
    public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, IEnumerable<string>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors?.ToArray();
    }

    public HttpStatusCode StatusCode { get; }
    public IReadOnlyCollection<string>? Errors { get; }
}
