using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

public static class HttpClientConfiguration
{
    public static HttpClient CreateClient()
    {
        var handler = new HttpClientHandler();
        var clientCertificate = new X509Certificate2("certs/ClientCert.pfx", "password");

        handler.ClientCertificates.Add(clientCertificate);
        //handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        // Validate server certificate
        // This 3 lines are optional  
        // but shows that client will verify the certificate of the server as well making it more secure
        //handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
        //{
        //    return cert.Issuer == "CN=ServerRootCA";
        //};

        return new HttpClient(handler);
    }
}
