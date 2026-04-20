using System.Text.Json;

namespace EsperancaSolidaria.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var traceId = context.TraceIdentifier;
            var correlationId = context.Request.Headers.TryGetValue("X-Correlation-ID", out var id) ? id.ToString() : Guid.NewGuid().ToString();

            _logger.LogError(ex,
                "Erro: {Message} | TraceId: {TraceId} | CorrelationId: {CorrelationId}",
                ex.Message, traceId, correlationId);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                Message = _env.IsDevelopment()
                    ? ex.Message
                    : "Ocorreu um erro inesperado. Tente novamente mais tarde.",
                Details = _env.IsDevelopment() ? ex.InnerException?.Message ?? ex.Message : null,
                StackTrace = _env.IsDevelopment() ? ex.StackTrace : null,
                TraceId = traceId
            };

            var json = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(json);
        }
    }
}

public class ErrorResponse
{
    public string Message { get; set; }
    public string? Details { get; set; }
    public string? StackTrace { get; set; }
    public string TraceId { get; set; }
}