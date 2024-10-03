using System;
using System.IO;
using System.Diagnostics;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using System.Text;
using Org.BouncyCastle.Bcpg.OpenPgp;

public class Program
{
    public static async Task Main(string[] args)
    {
        var serviceProvider = BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        string todayDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        string logLoc = $"/tmp/{todayDate}-asfritecxbackup.log";
        string backupLoc = $"/tmp/backup";
        string metadataLoc = $"{backupLoc}/{todayDate}-metadata.log";
        string mainBackup = $"{todayDate}-asfritecxbackup.tar.gz";
        string encryptedBackup = $"/tmp/{mainBackup}.gpg";

        try
        {
            Directory.CreateDirectory(backupLoc);
            await File.WriteAllTextAsync(logLoc, string.Empty);
            logger.LogInformation("Backup process started at {Time}", DateTime.Now);

            // Retrieve NGINX Metadata
            await ExecuteCommandAsync("nginx", "-V", metadataLoc);

            // Get paths to backup from configuration
            var pathsToBackup = configuration.GetSection("PathsToBackup").Get<string[]>();

            foreach (var path in pathsToBackup)
            {
                var backupNamePrefix = path.TrimStart('/').Replace("/", "-") + "backup";
                await BackupDirectoryAsync(path, backupNamePrefix, backupLoc, logLoc, configuration, logger);
            }

            // Compress the backup
            CompressDirectory(backupLoc, $"/tmp/{mainBackup}");

            // Encrypt the backup
            var publicKeyPath = configuration["PublicKeyPath"];
            if (!string.IsNullOrEmpty(publicKeyPath))
            {
                EncryptFileWithPgp($"/tmp/{mainBackup}", encryptedBackup, publicKeyPath, logger);
            }
            else
            {
                throw new Exception("Public key path is not specified in the configuration.");
            }

            // Upload via SFTP
            await UploadViaSftpAsync(encryptedBackup, configuration, logger);

            logger.LogInformation("Backup process completed successfully at {Time}", DateTime.Now);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during the backup process. The backup will not be uploaded.");
        }
        finally
        {
            // Cleanup
            CleanupFiles(new[] { $"/tmp/{mainBackup}", encryptedBackup, backupLoc, "/tmp/ghost" });
            logger.LogInformation("Cleanup completed at {Time}", DateTime.Now);
        }
    }


    private static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddLogging(configure => configure.AddConsole());
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build());

        return services.BuildServiceProvider();
    }

    private static async Task ExecuteCommandAsync(string fileName, string arguments, string outputPath)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();

        var output = await process.StandardOutput.ReadToEndAsync();
        await File.WriteAllTextAsync(outputPath, output);
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new Exception($"Command {fileName} {arguments} failed with exit code {process.ExitCode}");
        }
    }

    private static async Task BackupDirectoryAsync(string sourceDir, string backupNamePrefix, string backupLoc, string logLoc, IConfiguration config, ILogger logger)
    {
        string todayDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        string backupFileName = $"{todayDate}-{backupNamePrefix}.tar";

        logger.LogInformation("Bundling {BackupFileName}", backupFileName);

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "tar",
                Arguments = $"-cf {backupLoc}/{backupFileName} {sourceDir}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        await File.AppendAllTextAsync(logLoc, output);
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new Exception($"Error bundling {backupFileName}. Check logs at {logLoc}");
        }

        logger.LogInformation("{BackupFileName} is bundled!", backupFileName);
    }

    private static void CompressDirectory(string sourceDir, string destFileName)
    {
        if (File.Exists(destFileName))
        {
            File.Delete(destFileName);
        }

        ZipFile.CreateFromDirectory(sourceDir, destFileName);
    }

    private static void EncryptFileWithPgp(string inputFilePath, string outputFilePath, string publicKeyPath, ILogger logger)
    {
        try
        {
            PgpEncryptionHelper.EncryptFile(inputFilePath, outputFilePath, publicKeyPath);
            logger.LogInformation("File encrypted successfully: {FilePath}", outputFilePath);
        }
        catch (Exception ex)
        {
            throw new Exception("PGP encryption failed", ex);
        }
    }



    private static async Task UploadViaSftpAsync(string filePath, IConfiguration config, ILogger logger)
    {
        var host = config["Sftp:Host"];
        var username = config["Sftp:Username"];
        var password = config["Sftp:Password"];
        var remotePath = config["Sftp:RemotePath"];

        try
        {
            using var client = new SftpClient(host, username, password);
            client.Connect();

            using var fileStream = new FileStream(filePath, FileMode.Open);
            await Task.Run(() => client.UploadFile(fileStream, $"{remotePath}/{Path.GetFileName(filePath)}"));

            client.Disconnect();

            logger.LogInformation("{FilePath} successfully uploaded!", filePath);
        }
        catch (Exception ex)
        {
            throw new Exception("SFTP upload failed", ex);
        }
    }

    private static void CleanupFiles(string[] paths)
    {
        foreach (var path in paths)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            else if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
