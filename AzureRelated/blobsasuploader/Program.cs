using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Newtonsoft.Json.Linq;

namespace blobsasuploader
{
    class Program
    {
        static async Task Main(string[] args)
        {

            if (args.Length > 0 && args[0].ToLower() == "config")
            {
                ConfigureAppSettings();
                return;
            }

            var host = CreateHostBuilder(args).Build();
            var uploader = host.Services.GetRequiredService<BlobUploader>();
            var configuration = host.Services.GetRequiredService<IConfiguration>();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            var containerName = configuration["UploadSettings:ContainerName"];
            var toUploadFolderPath = Path.Combine(AppContext.BaseDirectory, "ToUpload");

            try
            {
                if (!Directory.Exists(toUploadFolderPath))
                {
                    Directory.CreateDirectory(toUploadFolderPath);
                    logger.LogInformation("Created 'ToUpload' folder at {Path}", toUploadFolderPath);
                }

                var files = Directory.GetFiles(toUploadFolderPath);
                if (files.Length == 0)
                {
                    logger.LogInformation("No files found in 'ToUpload' folder.");
                }

                foreach (var file in files)
                {
                    try
                    {
                        await uploader.UploadFileAsync(file, containerName);
                        logger.LogInformation("Successfully uploaded {FileName}", file);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error uploading file {FileName}", file);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during the upload process.");
            }

            logger.LogInformation("Program exiting in 10 seconds...");
            Thread.Sleep(10000);
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<BlobUploader>();
                    services.AddLogging(configure => configure.AddConsole());
                });

        static void ConfigureAppSettings()
        {
            string appSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            string connectionString = PromptUser("Enter your Azure Blob Storage connection string:");
            string containerName = PromptUser("Enter your container name:");

            var appSettings = new JObject(
                new JProperty("ConnectionStrings", new JObject(
                    new JProperty("AzureBlobStorage", connectionString))),
                new JProperty("UploadSettings", new JObject(
                    new JProperty("ContainerName", containerName))),
                new JProperty("Logging", new JObject(
                    new JProperty("LogLevel", new JObject(
                        new JProperty("Default", "Information"),
                        new JProperty("Microsoft", "Warning"),
                        new JProperty("Microsoft.Hosting.Lifetime", "Information"))))));

            File.WriteAllText(appSettingsPath, appSettings.ToString());
            Console.WriteLine("Configuration saved to appsettings.json.");
        }

        static string PromptUser(string message)
        {
            Console.Write(message + " ");
            return Console.ReadLine();
        }
    }
}
