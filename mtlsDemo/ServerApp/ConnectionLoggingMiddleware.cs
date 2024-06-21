
public class ConnectionLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ConnectionLoggingMiddleware> _logger;

    public ConnectionLoggingMiddleware(RequestDelegate next, ILogger<ConnectionLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        _logger.LogInformation($"Incoming connection from IP: {context.Connection.RemoteIpAddress}:{context.Connection.RemotePort} to {context.Request.Path.ToString()} {context.Request.Method} {context.Request.Protocol.ToString()} \n");

        if (context.Connection.ClientCertificate == null)
        {
            _logger.LogInformation($"Client Cert not presented");
        }
        else 
        {
            _logger.LogInformation($"Client Cert presented with Thumbprint : {context.Connection.ClientCertificate.Thumbprint}");
        }

        await _next(context);
    }
}
