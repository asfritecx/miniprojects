using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;

namespace ServerApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();

            // Configure logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // Load configuration values
            var certLocation = builder.Configuration["Kestrel:CertLocation"];
            var certPassword = builder.Configuration["Kestrel:CertPassword"];
            var port = int.Parse(builder.Configuration["Kestrel:Port"]);

            // Configure Kestrel to use the server certificate for mTLS
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Any, port, listenOptions =>
                {
                    listenOptions.UseHttps(certLocation, certPassword, httpsOptions =>
                    {
                        httpsOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                        httpsOptions.ClientCertificateValidation = (cert, chain, errors) =>
                        {
                            var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

                            // Validate client certificate
                            if (errors != SslPolicyErrors.None)
                            {
                                logger.LogDebug("Client certificate validation failed: {errors}", errors);
                                return false;
                            }

                            // Check if the certificate is issued by a trusted CA
                            if (cert.Issuer != "CN=ClientRootCA")
                            {
                                logger.LogDebug("Client certificate issuer is not trusted: {issuer}", cert.Issuer);
                                return false;
                            }

                            // Additional checks
                            if (cert.NotBefore > DateTime.Now || cert.NotAfter < DateTime.Now)
                            {
                                logger.LogDebug("Client certificate is not within its validity period: {notBefore} - {notAfter}", cert.NotBefore, cert.NotAfter);
                                return false;
                            }

                            // Check certificate thumbprint
                            if (cert.Thumbprint != "F0716F80BB221CBC8D0A3348A677B650C72BA90E")
                            {
                                logger.LogDebug("Client certificate thumbprint is not valid: {thumbprint}", cert.Thumbprint);
                                return false;
                            }

                            // Optionally validate the certificate chain
                            bool isChainValid = chain.Build(cert);
                            if (!isChainValid)
                            {
                                logger.LogDebug("Client certificate chain validation failed.");
                                return false;
                            }

                            return true;
                        };
                    });
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
