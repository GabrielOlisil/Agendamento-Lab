namespace Agendamentos.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context )
    {
        try
        {
            var user = context.User;
            var userName = user?.Identity?.IsAuthenticated == true ? user.Identity.Name : "Anônimo";

            var data = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4));
            _logger.LogInformation(
                "Requisição recebida: {Method} {Path} por {User} às {Timestamp}",
                context.Request.Method,
                context.Request.Path,
                userName,
                data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao logar a requisição.");
        }
        
        await _next(context);
    }
}