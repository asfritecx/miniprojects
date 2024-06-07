using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
            var serverURL = configuration["ConnectionSettings:ServerURL"];
            var port = configuration["ConnectionSettings:Port"];
            var clientCertPath = configuration["ConnectionSettings:ClientCertificatePath"];
            var clientCertPassword = configuration["ConnectionSettings:ClientCertificatePassword"];
            var clientId = configuration["ClientSettings:ClientId"];
            var clientSecret = configuration["ClientSettings:ClientSecret"];

            // Create service collection and configure our services
            var services = new ServiceCollection();
            ConfigureServices(services, serverURL, port, clientCertPath, clientCertPassword, clientId, clientSecret);

            // Generate a provider
            var serviceProvider = services.BuildServiceProvider();

            // Use the service
            await UseHttpClient(serviceProvider);
        }

        private static void ConfigureServices(IServiceCollection services, string serverURL, string port, string clientCertPath, string clientCertPassword, string clientId, string clientSecret)
        {
            // Load client certificate
            var clientCertificate = new X509Certificate2(clientCertPath, clientCertPassword);

            // Configure logging
            services.AddLogging(configure =>
            {
                configure.ClearProviders();
                configure.AddConsole();
                configure.SetMinimumLevel(LogLevel.Debug);
            });

            // Configure the HttpClient with the base address and client certificate
            services.AddHttpClient("MyClient", client =>
            {
                client.BaseAddress = new Uri($"{serverURL}:{port}/");
                client.DefaultRequestHeaders.Add("clientId", clientId);
                client.DefaultRequestHeaders.Add("clientSecret", clientSecret);
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
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            var clientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var client = clientFactory.CreateClient("MyClient");

            try
            {
                // Send GET request
                var getResponse = await client.GetAsync("api/values");
                getResponse.EnsureSuccessStatusCode();
                var getContent = await getResponse.Content.ReadAsStringAsync();
                Console.WriteLine(getContent);
                logger.LogInformation("GET request succeeded with response: {Content}", getContent);

                // Send POST request
                var postContent = new StringContent(JsonConvert.SerializeObject("Hello"), Encoding.UTF8, "application/json");
                var postResponse = await client.PostAsync("api/values", postContent);
                postResponse.EnsureSuccessStatusCode();
                var postResponseContent = await postResponse.Content.ReadAsStringAsync();
                Console.WriteLine(postResponseContent);
                logger.LogInformation("POST request succeeded with response: {Content}", postResponseContent);

                // Send POST request to restricted endpoint
                var restrictedResponse = await client.PostAsync("api/values/restricted", null);
                restrictedResponse.EnsureSuccessStatusCode();
                var restrictedResponseContent = await restrictedResponse.Content.ReadAsStringAsync();
                Console.WriteLine(restrictedResponseContent);
                logger.LogInformation("POST to restricted endpoint succeeded with response: {Content}", restrictedResponseContent);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while making the request");
            }
        }
    }
}
