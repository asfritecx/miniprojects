using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace blobsasuploader
{
    class Program
    {
        static async Task Main(string[] args)
        {
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
    }

    public class BlobUploader
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BlobUploader> _logger;

        public BlobUploader(IConfiguration configuration, ILogger<BlobUploader> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task UploadFileAsync(string filePath, string containerName)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                _logger.LogError("Invalid file path provided.");
                throw new ArgumentException("File path is invalid or file does not exist.");
            }

            string connectionString = _configuration.GetConnectionString("AzureBlobStorage");
            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogError("Azure Blob Storage connection string is missing.");
                throw new InvalidOperationException("Azure Blob Storage connection string is not configured.");
            }

            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                await containerClient.CreateIfNotExistsAsync();

                string blobName = Path.GetFileName(filePath);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                using FileStream uploadFileStream = File.OpenRead(filePath);
                await blobClient.UploadAsync(uploadFileStream, overwrite: true);
                uploadFileStream.Close();

                _logger.LogInformation("File uploaded successfully to container {ContainerName}", containerName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to Azure Blob Storage.");
                throw;
            }
        }
    }
}
