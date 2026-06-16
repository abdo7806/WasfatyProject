using Serilog.Context;
using System.Net;
using System.Security.Claims;

namespace Wasfaty.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
            var userEmail = context.User.FindFirstValue(ClaimTypes.Email) ?? "unknown";

            _logger.LogError(exception,
                "Unhandled exception | Path: {Path} | UserId: {UserId} | UserEmail: {UserEmail} | Method: {Method}",
                context.Request.Path,
                userId,
                userEmail,
                context.Request.Method);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                status = context.Response.StatusCode,
                title = "Internal Server Error",
                detail = "حدث خطأ داخلي. يرجى المحاولة لاحقاً.",
                traceId = context.TraceIdentifier,
                path = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }

}