using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        var serviceProvider = ConfigureServices();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        string storageAccountName = configuration["AzureBlob:StorageAccountName"];
        string containerName = configuration["AzureBlob:ContainerName"];
        string sasToken = configuration["AzureBlob:SasToken"];

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("Select an action:");
            Console.WriteLine("1. List latest backups");
            Console.WriteLine("2. Generate restore script");
            Console.WriteLine("9. Configure app settings");
            Console.WriteLine("3. Exit");
            string actionChoice = Console.ReadLine();

            switch (actionChoice)
            {
                case "1":
                    Console.WriteLine("Enter the database name filter:");
                    string databaseNameFilter = Console.ReadLine();
                    await ListLatestBackups(storageAccountName, containerName, sasToken, databaseNameFilter);
                    break;
                case "2":
                    Console.WriteLine("Enter the name of the database for which you want to generate a restore script (Press Enter for all databases):");
                    string databaseName = Console.ReadLine();

                    if (string.IsNullOrEmpty(databaseName))
                    {
                        // Generate restore script for all databases
                        string outputFilePath = "restore_all_databases.sql"; // Define the path to your output file
                        await GenerateRestoreScriptForAllDatabases(storageAccountName, containerName, sasToken, outputFilePath);
                    }
                    else
                    {
                        // Get the latest backups
                        var latestBackups = await GetLatestBackups(storageAccountName, containerName, sasToken, databaseName);

                        // Generate the restore script
                        string outputFilePath = $"restore_{databaseName}.sql"; // Define the path to your output file
                        using (var writer = new StreamWriter(outputFilePath))
                        {
                            GenerateRestoreScript(databaseName, latestBackups, storageAccountName, containerName, writer);
                        }

                        Console.WriteLine($"Restore script for {databaseName} has been generated and saved to {outputFilePath}");
                    }
                    break;
                case "9":
                    ConfigureAppSettings();
                    break;
                case "3":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please choose a valid option.");
                    break;
            }

            if (!exit)
            {
                Console.WriteLine("Press any key to continue or 'q' to quit...");
                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.Q)
                {
                    exit = true;
                }
                Console.Clear(); // Clear the console for the next operation
            }
        }

        Console.WriteLine("Exiting the application. Goodbye!");
    }

    static ServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(builder => builder.AddConsole());
        serviceCollection.AddSingleton<IConfiguration>(new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build());

        return serviceCollection.BuildServiceProvider();
    }

    static async Task<Dictionary<string, BackupInfo>> GetLatestBackups(string storageAccountName, string containerName, string sasToken, string databaseNameFilter)
    {
        var latestBackups = new Dictionary<string, BackupInfo>();
        var blobServiceClient = new BlobServiceClient(new Uri($"https://{storageAccountName}.blob.core.windows.net?{sasToken}"));
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        {
            if (blobItem.Name.Contains(databaseNameFilter, StringComparison.OrdinalIgnoreCase))
            {
                var blobProperties = blobItem.Properties;
                string databaseName = ExtractDatabaseName(blobItem.Name);
                //Console.WriteLine("Fetching latest backups for: " + databaseName);
                string backupType = DetermineBackupType(blobItem.Name);

                if (!latestBackups.ContainsKey(databaseName))
                {
                    latestBackups[databaseName] = new BackupInfo();
                }

                var backupInfo = latestBackups[databaseName];
                DateTime lastModified = blobProperties.LastModified.GetValueOrDefault().UtcDateTime;

                if (backupType.Equals("Full", StringComparison.OrdinalIgnoreCase))
                {
                    // If this is the most recent full backup, clear existing parts and add the new one
                    if (lastModified > backupInfo.FullBackupLastModified)
                    {
                        backupInfo.FullBackupParts.Clear();
                        backupInfo.FullBackupLastModified = lastModified;
                    }

                    if (lastModified == backupInfo.FullBackupLastModified)
                    {
                        backupInfo.FullBackupParts.Add(blobItem.Name);
                    }
                }
                else if (backupType.Equals("Differential", StringComparison.OrdinalIgnoreCase))
                {
                    // If this is the most recent differential backup, ensure a full backup exists
                    if (backupInfo.FullBackupParts.Count > 0)
                    {
                        if (lastModified > backupInfo.DifferentialBackupLastModified)
                        {
                            backupInfo.DifferentialBackupParts.Clear();
                            backupInfo.DifferentialBackupLastModified = lastModified;
                        }

                        if (lastModified == backupInfo.DifferentialBackupLastModified)
                        {
                            backupInfo.DifferentialBackupParts.Add(blobItem.Name);
                        }
                    }
                }
            }
        }

        return latestBackups;
    }

    static void GenerateRestoreScript(string databaseName, Dictionary<string, BackupInfo> latestBackups, string storageAccountName, string containerName, StreamWriter writer)
    {
        if (!latestBackups.ContainsKey(databaseName))
        {
            Console.WriteLine($"No backups found for {databaseName}.");
            return;
        }

        var backupInfo = latestBackups[databaseName];

        if (backupInfo.FullBackupParts.Count == 0)
        {
            Console.WriteLine("No Full Backup found.");
            return;
        }

        writer.WriteLine("USE [master]");
        writer.WriteLine($"--{databaseName}");
        writer.WriteLine($"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");

        writer.WriteLine($"RESTORE DATABASE [{databaseName}] FROM");
        for (int i = 0; i < backupInfo.FullBackupParts.Count; i++)
        {
            string part = backupInfo.FullBackupParts[i];
            if (i == backupInfo.FullBackupParts.Count - 1)
            {
                writer.WriteLine($"    URL = N'https://{storageAccountName}.blob.core.windows.net/{containerName}/{part}'");
            }
            else
            {
                writer.WriteLine($"    URL = N'https://{storageAccountName}.blob.core.windows.net/{containerName}/{part}',");
            }
        }

        if (backupInfo.DifferentialBackupParts.Count > 0)
        {
            writer.WriteLine("    WITH FILE = 1, NORECOVERY, NOUNLOAD, REPLACE, STATS = 5");

            writer.WriteLine($"RESTORE DATABASE [{databaseName}] FROM");
            for (int i = 0; i < backupInfo.DifferentialBackupParts.Count; i++)
            {
                string part = backupInfo.DifferentialBackupParts[i];
                if (i == backupInfo.DifferentialBackupParts.Count - 1)
                {
                    writer.WriteLine($"    URL = N'https://{storageAccountName}.blob.core.windows.net/{containerName}/{part}'");
                }
                else
                {
                    writer.WriteLine($"    URL = N'https://{storageAccountName}.blob.core.windows.net/{containerName}/{part}',");
                }
            }
            writer.WriteLine("    WITH FILE = 1, RECOVERY, NOUNLOAD, STATS = 5");
        }
        else
        {
            writer.WriteLine("    WITH FILE = 1, RECOVERY, NOUNLOAD, REPLACE, STATS = 5");
        }

        writer.WriteLine($"ALTER DATABASE [{databaseName}] SET MULTI_USER");
        writer.WriteLine("GO");
        writer.WriteLine(); // Add a blank line for readability
    }

    static async Task GenerateRestoreScriptForAllDatabases(string storageAccountName, string containerName, string sasToken, string outputFilePath)
    {
        var latestBackups = await GetLatestBackups(storageAccountName, containerName, sasToken, string.Empty);

        using (var writer = new StreamWriter(outputFilePath))
        {
            writer.WriteLine("USE [master]");

            foreach (var database in latestBackups.Keys)
            {
                var backupInfo = latestBackups[database];

                // Ensure there is at least one full backup to proceed
                if (backupInfo.FullBackupParts.Count > 0)
                {
                    writer.WriteLine($"--{database}");
                    writer.WriteLine($"ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");

                    writer.WriteLine($"RESTORE DATABASE [{database}] FROM");
                    for (int i = 0; i < backupInfo.FullBackupParts.Count; i++)
                    {
                        string part = backupInfo.FullBackupParts[i];
                        if (i == backupInfo.FullBackupParts.Count - 1)
                        {
                            writer.WriteLine($"    URL = N'https://{storageAccountName}.blob.core.windows.net/{containerName}/{part}'");
                        }
                        else
                        {
                            writer.WriteLine($"    URL = N'https://{storageAccountName}.blob.core.windows.net/{containerName}/{part}',");
                        }
                    }

                    if (backupInfo.DifferentialBackupParts.Count > 0)
                    {
                        writer.WriteLine("    WITH FILE = 1, NORECOVERY, NOUNLOAD, REPLACE, STATS = 5");

                        writer.WriteLine($"RESTORE DATABASE [{database}] FROM");
                        for (int i = 0; i < backupInfo.DifferentialBackupParts.Count; i++)
                        {
                            string part = backupInfo.DifferentialBackupParts[i];
                            if (i == backupInfo.DifferentialBackupParts.Count - 1)
                            {
                                writer.WriteLine($"    URL = N'https://{storageAccountName}.blob.core.windows.net/{containerName}/{part}'");
                            }
                            else
                            {
                                writer.WriteLine($"    URL = N'https://{storageAccountName}.blob.core.windows.net/{containerName}/{part}',");
                            }
                        }
                        writer.WriteLine("    WITH FILE = 1, RECOVERY, NOUNLOAD, STATS = 5");
                    }
                    else
                    {
                        writer.WriteLine("    WITH FILE = 1, RECOVERY, NOUNLOAD, REPLACE, STATS = 5");
                    }

                    writer.WriteLine($"ALTER DATABASE [{database}] SET MULTI_USER");
                    writer.WriteLine("GO");
                    writer.WriteLine(); // Add a blank line for readability between database sections
                }
            }
        }

        Console.WriteLine($"Restore script has been generated and saved to {outputFilePath}");
    }

    static async Task ListLatestBackups(string storageAccountName, string containerName, string sasToken, string databaseNameFilter)
    {
        var latestBackups = await GetLatestBackups(storageAccountName, containerName, sasToken, databaseNameFilter);

        foreach (var backup in latestBackups)
        {
            Console.WriteLine($"Database Name: {backup.Key}");

            if (backup.Value.FullBackupParts.Count > 0)
            {
                Console.WriteLine($"Latest Full Backup Parts (Last Modified: {backup.Value.FullBackupLastModified}):");
                backup.Value.FullBackupParts.ForEach(p => Console.WriteLine(p));
            }
            else
            {
                Console.WriteLine("No Full Backup found.");
            }

            if (backup.Value.FullBackupParts.Count > 0 && backup.Value.DifferentialBackupParts.Count > 0)
            {
                Console.WriteLine($"Latest Differential Backup Parts (Last Modified: {backup.Value.DifferentialBackupLastModified}):");
                backup.Value.DifferentialBackupParts.ForEach(p => Console.WriteLine(p));
            }
            else if (backup.Value.DifferentialBackupParts.Count == 0)
            {
                Console.WriteLine("No Differential Backup found.");
            }

            Console.WriteLine("----------");
        }
    }

    static void ConfigureAppSettings()
    {
        var configFilePath = "appsettings.json";
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(configFilePath)
            .Build();

        Console.WriteLine("Current App Settings:");
        string currentStorageAccountName = configuration["AzureBlob:StorageAccountName"];
        string currentContainerName = configuration["AzureBlob:ContainerName"];
        string currentSasToken = configuration["AzureBlob:SasToken"];

        Console.WriteLine($"1. Storage Account Name: {currentStorageAccountName}");
        Console.WriteLine($"2. Container Name: {currentContainerName}");
        Console.WriteLine($"3. SAS Token: {currentSasToken}");
        Console.WriteLine();

        Console.WriteLine("Enter new values for the following settings or press Enter to keep the current value.");

        Console.Write("Storage Account Name: ");
        string newStorageAccountName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(newStorageAccountName))
        {
            newStorageAccountName = currentStorageAccountName;
        }

        Console.Write("Container Name: ");
        string newContainerName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(newContainerName))
        {
            newContainerName = currentContainerName;
        }

        Console.Write("SAS Token: ");
        string newSasToken = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(newSasToken))
        {
            newSasToken = currentSasToken;
        }

        var updatedConfig = new
        {
            AzureBlob = new
            {
                StorageAccountName = newStorageAccountName,
                ContainerName = newContainerName,
                SasToken = newSasToken
            }
        };

        var json = System.Text.Json.JsonSerializer.Serialize(updatedConfig, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(configFilePath, json);

        Console.WriteLine("App settings have been updated. The application will now restart to apply the changes.");

        // Restart the application
        RestartApplication();
    }

    static void RestartApplication()
    {
        var fileName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        var startInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = fileName,
            Arguments = string.Join(" ", Environment.GetCommandLineArgs()),
            UseShellExecute = true
        };
        System.Diagnostics.Process.Start(startInfo);
        Environment.Exit(0); // Terminate the current process
    }

    static string ExtractDatabaseName(string blobName)
    {
        // Assuming the format is YYYYMMDDHHMM_DatabaseName_BackupType_PartX.bak
        var parts = blobName.Split('_');
        return parts[1]; // DatabaseName is the second part
    }

    static string DetermineBackupType(string blobName)
    {
        // Assuming BackupType is the third part and could be 'F' for Full or 'D' for Differential
        var parts = blobName.Split('_');
        return parts[2].StartsWith("F") ? "Full" : "Differential";
    }
}

class BackupInfo
{
    public List<string> FullBackupParts { get; private set; } = new List<string>();
    public DateTime FullBackupLastModified { get; set; }  // Ensure the setter is public

    public List<string> DifferentialBackupParts { get; private set; } = new List<string>();
    public DateTime DifferentialBackupLastModified { get; set; }  // Ensure the setter is public

    public void AddBackupPart(string backupType, string partName, DateTime lastModified)
    {
        if (backupType.Equals("Full", StringComparison.OrdinalIgnoreCase))
        {
            FullBackupParts.Add(partName);
            if (lastModified > FullBackupLastModified)
            {
                FullBackupLastModified = lastModified;
            }
        }
        else if (backupType.Equals("Differential", StringComparison.OrdinalIgnoreCase))
        {
            DifferentialBackupParts.Add(partName);
            if (lastModified > DifferentialBackupLastModified)
            {
                DifferentialBackupLastModified = lastModified;
            }
        }
    }
}