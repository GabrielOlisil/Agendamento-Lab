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
        // ===================================================
        // CÓDIGO QUE EXECUTA ANTES DO ENDPOINT
        // ===================================================
        try
        {
            // Usando o sistema de logging estruturado do .NET
            // Isso é mais eficiente e permite buscas melhores em sistemas de log
            
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
        
        // Chama a próxima "estação" na linha de montagem (pipeline).
        // Se esta linha for removida, a requisição para aqui e nunca chega ao controller!
        await _next(context);
        
        // ===================================================
        // CÓDIGO QUE EXECUTA DEPOIS DO ENDPOINT (na volta)
        // ===================================================
        // _logger.LogInformation("Requisição finalizada com status {StatusCode}", context.Response.StatusCode);
    }
}