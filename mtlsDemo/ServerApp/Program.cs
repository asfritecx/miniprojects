using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Net;

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
                        httpsOptions.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                    });
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            app.UseHttpsRedirection();

            // Use connection logging middleware
            app.UseMiddleware<ConnectionLoggingMiddleware>();

            // Use custom client certificate middleware
            app.UseMiddleware<CustomClientCertificateMiddleware>();

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
