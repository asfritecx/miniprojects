using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClientApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Read connection settings
            var serverDNS = configuration["ConnectionSettings:ServerDNS"];
            var port = configuration["ConnectionSettings:Port"];
            var clientCertPath = configuration["ConnectionSettings:ClientCertificatePath"];
            var clientCertPassword = configuration["ConnectionSettings:ClientCertificatePassword"];

            // Create service collection and configure our services
            var services = new ServiceCollection();
            ConfigureServices(services, serverDNS, port, clientCertPath, clientCertPassword);

            // Generate a provider
            var serviceProvider = services.BuildServiceProvider();

            // Use the service
            await UseHttpClient(serviceProvider);
        }

        private static void ConfigureServices(IServiceCollection services, string serverDNS, string port, string clientCertPath, string clientCertPassword)
        {
            // Load client certificate
            var clientCertificate = new X509Certificate2(clientCertPath, clientCertPassword);

            // Configure the HttpClient with the base address and client certificate
            services.AddHttpClient("MyClient", client =>
            {
                client.BaseAddress = new Uri($"{serverDNS}:{port}/");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    ClientCertificates = { clientCertificate },
                };
            });
        }

        private static async Task UseHttpClient(IServiceProvider serviceProvider)
        {
            var clientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var client = clientFactory.CreateClient("MyClient");

            var response = await client.GetAsync("api/values");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        }
    }
}
