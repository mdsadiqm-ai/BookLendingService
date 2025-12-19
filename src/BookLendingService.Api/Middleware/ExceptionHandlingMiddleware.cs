using Microsoft.EntityFrameworkCore;

namespace BookLendingService.Api.Middleware;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception ex)
    {
        var correlationId = GetCorrelationId(context);

        var (statusCode, errorCode, message) = ex switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "not_found", ex.Message),
            ArgumentException => (StatusCodes.Status400BadRequest, "bad_request", ex.Message),
            InvalidOperationException => (StatusCodes.Status409Conflict, "conflict", ex.Message),
            DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "concurrency_conflict", "Concurrency conflict, please retry"),
            _ => (StatusCodes.Status500InternalServerError, "server_error", "Unexpected error")
        };

        if (statusCode >= 500)
        {
            _logger.LogError(ex,
                $"Unhandled exception. statusCode={statusCode} errorCode={errorCode} correlationId={correlationId} path={context.Request.Path}",
                statusCode, errorCode, correlationId, context.Request.Path);
        }
        else
        {
            _logger.LogWarning(ex,
                $"Handled exception. statusCode={statusCode} errorCode={errorCode} correlationId={correlationId} path={context.Request.Path}",
                statusCode, errorCode, correlationId, context.Request.Path);
        }

        if (context.Response.HasStarted)
        {
            _logger.LogWarning($"Response already started, cannot write error payload. correlationId={correlationId}", correlationId);
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var payload = new ErrorResponse(
            Error: message,
            ErrorCode: errorCode,
            TraceId: correlationId
        );

        await context.Response.WriteAsJsonAsync(payload);
    }

    private static string GetCorrelationId(HttpContext context)
    {
        if (
            context.Items.TryGetValue(CorrelationIdMiddleware.HeaderName, out var value) 
            && value is string s 
            && !string.IsNullOrWhiteSpace(s)
            )
            return s;

        return context.TraceIdentifier;
    }

    private sealed record ErrorResponse(string Error, string ErrorCode, string TraceId);
}
