using System;
using System.Drawing;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
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
            var apiEndpoint = configuration["ConnectionSettings:APIPath"];
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
            await UseHttpClient(serviceProvider, apiEndpoint);

            // Keep the console window open
            while (true)
            {
                ColorConsole.WriteLine("Would you like to exit the application? (Y/N) : ", ConsoleColor.Yellow);
                var input = Console.ReadLine();
                if (input.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }
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
            services.AddHttpClient("mtlsClient", client =>
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

            // Configure the HttpClient for the normal endpoint without client certificate
            services.AddHttpClient("NormalClient", client =>
            {
                client.BaseAddress = new Uri($"{serverURL}:{port}/");
            });
        }

        private static async Task UseHttpClient(IServiceProvider serviceProvider, string url)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            var clientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            // Create the HTTP Client with the client certificate to present the cert to the server
            var client = clientFactory.CreateClient("mtlsClient");
            // Creates a normal HTTP Client without the client certificate
            var normalClient = clientFactory.CreateClient("NormalClient");

            try
            {
                // Send GET request to normal endpoint
                var normalResponse = await normalClient.GetAsync($"{url}");
                normalResponse.EnsureSuccessStatusCode();
                var normalContent = await normalResponse.Content.ReadAsStringAsync();
                ColorConsole.WriteLog($"GET request to normal endpoint succeeded with response: {normalContent}", LogLevel.Information);
            }
            catch (Exception ex)
            {
                ColorConsole.WriteLog($"An error occurred while making the request to normal endpoint \nError :  {ex.Message} \nInner Exception : {ex.InnerException.StackTrace.ToString()}", LogLevel.Error);
            }

            try
            {
                // Send GET request
                var getResponse = await client.GetAsync($"{url}");
                getResponse.EnsureSuccessStatusCode();
                var getContent = await getResponse.Content.ReadAsStringAsync();
                ColorConsole.WriteLog($"GET request succeeded with response: {getContent}", LogLevel.Information);
            }
            catch (Exception ex)
            {
                ColorConsole.WriteLog($"An error occurred while making the request to the client certificate endpoint at api/values \n Error :  {ex.Message} \nInner Exception : {ex.InnerException.Message}", LogLevel.Error);
            }

            try { 
            // Send POST request
            var postContent = new StringContent(JsonConvert.SerializeObject("Hello"), Encoding.UTF8, "application/json");
            var postResponse = await client.PostAsync("api/values", postContent);
            postResponse.EnsureSuccessStatusCode();
            var postResponseContent = await postResponse.Content.ReadAsStringAsync();
            ColorConsole.WriteLog($"POST request succeeded with response: {postResponseContent}", LogLevel.Information);
            }
            catch (Exception ex) {
                ColorConsole.WriteLog($"An error occurred while making the request to the client certificate endpoint at api/values \n Error :  {ex.Message} \nInner Exception : {ex.InnerException.Message}", LogLevel.Error);
            }

            try {
                // Send POST request to restricted endpoint
                var restrictedResponse = await client.PostAsync("api/values/restricted", null);
                restrictedResponse.EnsureSuccessStatusCode();
                var restrictedResponseContent = await restrictedResponse.Content.ReadAsStringAsync();
                ColorConsole.WriteLog($"POST to restricted endpoint succeeded with response: {restrictedResponseContent}", LogLevel.Information);
            } catch (Exception ex)
            {
                ColorConsole.WriteLog($"An error occurred while making the request to the client certificate endpoint at api/values/restricted \n Error :  {ex.Message} \nInner Exception : {ex.InnerException.Message}", LogLevel.Error);
            }

        }
    }
}

