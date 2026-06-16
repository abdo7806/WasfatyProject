using Serilog.Context;

namespace Wasfaty.API.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                                ?? Guid.NewGuid().ToString();

            context.Response.Headers.Append("X-Correlation-Id", correlationId);

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                _logger.LogDebug("Request started: {Method} {Path} with CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path,
                    correlationId);

                await _next(context);

                _logger.LogDebug("Request completed: {Method} {Path} with StatusCode: {StatusCode}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode);
            }
        }
    }
}
