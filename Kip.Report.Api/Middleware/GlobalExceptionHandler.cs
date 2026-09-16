using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Kip.Report.Domain.Exceptions;

namespace Kip.Report.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception: {Message}", exception.Message);

        var (statusCode, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message),

            ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),

            OperationCanceledException => (StatusCodes.Status499ClientClosedRequest, "Request canceled"),

            _ => (StatusCodes.Status500InternalServerError, "Internal error.")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
