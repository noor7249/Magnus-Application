using Magnus.API.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text.Json;

namespace Magnus.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing the request.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            AppException appException => (int)appException.StatusCode,
            SecurityTokenException => (int)HttpStatusCode.Unauthorized,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var response = exception switch
        {
            AppException appException => ApiResponse<object>.FailureResponse(
                appException.Message,
                appException.Errors,
                context.TraceIdentifier),
            _ => ApiResponse<object>.FailureResponse(
                "An unexpected error occurred.",
                new[] { exception.Message },
                context.TraceIdentifier)
        };

        var json = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(json);
    }
}
