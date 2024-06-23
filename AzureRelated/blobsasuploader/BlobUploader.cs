using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blobsasuploader
{
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
