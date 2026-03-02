using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MoneyTrace.Domain.Primitives;

namespace MoneyTrace.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var error = new Error("Server.InternalError", "An unexpected error occurred. Please try again later.");
        
        // In a real application, you might conditionally include the exception message based on environment
        
        await context.Response.WriteAsJsonAsync(error);
    }
}
