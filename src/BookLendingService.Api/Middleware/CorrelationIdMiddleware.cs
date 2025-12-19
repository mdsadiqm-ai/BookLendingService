namespace BookLendingService.Api.Middleware;

public sealed class CorrelationIdMiddleware : IMiddleware
{
    public const string HeaderName = "X-Correlation-Id";

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var incoming) &&
                            !string.IsNullOrWhiteSpace(incoming)
            ? incoming.ToString()
            : context.TraceIdentifier;

        context.Items[HeaderName] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        return next(context);
    }
}