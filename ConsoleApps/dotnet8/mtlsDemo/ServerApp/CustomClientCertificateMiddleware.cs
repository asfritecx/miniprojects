using System.Security.Cryptography.X509Certificates;

public class CustomClientCertificateMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomClientCertificateMiddleware> _logger;

    public CustomClientCertificateMiddleware(RequestDelegate next, ILogger<CustomClientCertificateMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api/values"))
        {
            var clientCertificate = await context.Connection.GetClientCertificateAsync();
            if (clientCertificate == null)
            {
                _logger.LogError("Client certificate is required for this endpoint.");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Client certificate is required.");
                return;
            }

            if (!ValidateCertificate(clientCertificate))
            {
                _logger.LogError("Invalid client certificate.");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Invalid client certificate.");
                return;
            }
        }

        await _next(context);
    }

    private bool ValidateCertificate(X509Certificate2 cert)
    {
        // This is where we check if the client has a valid cert that we trust
        if (cert.Issuer != "CN=ClientRootCA")
        {
            _logger.LogError("Client certificate issuer is not trusted: {issuer}", cert.Issuer);
            return false;
        }

        if (cert.NotBefore > DateTime.Now || cert.NotAfter < DateTime.Now)
        {
            _logger.LogError("Client certificate is not within its validity period: {notBefore} - {notAfter}", cert.NotBefore, cert.NotAfter);
            return false;
        }

        if (cert.Thumbprint != "F0716F80BB221CBC8D0A3348A677B650C72BA90E")
        {
            _logger.LogError("Client certificate thumbprint is not valid: {thumbprint}", cert.Thumbprint);
            return false;
        }

        // Optionally validate the certificate chain if it is a trusted certificate
        //var chain = new X509Chain();
        //bool isChainValid = chain.Build(cert);
        //if (!isChainValid)
        //{
        //    _logger.LogError("Client certificate chain validation failed.");
        //    return false;
        //}

        return true;
    }
}
