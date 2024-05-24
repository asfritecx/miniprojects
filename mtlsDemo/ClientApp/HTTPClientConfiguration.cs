using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

public static class HttpClientConfiguration
{
    public static HttpClient CreateClient()
    {
        var handler = new HttpClientHandler();
        var clientCertificate = new X509Certificate2("certs/ClientCert.pfx", "password");

        handler.ClientCertificates.Add(clientCertificate);
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
        {
            // Validate server certificate
            return cert.Issuer == "CN=ServerRootCA";
        };

        return new HttpClient(handler);
    }
}
