using System;
using System.IO;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;

public class PgpEncryptionHelper
{
    public static void EncryptFile(string inputFilePath, string outputFilePath, string publicKeyPath, bool armor = true, bool withIntegrityCheck = true)
    {
        using var outputStream = File.Create(outputFilePath);
        using var publicKeyStream = File.OpenRead(publicKeyPath);
        EncryptFile(inputFilePath, outputStream, publicKeyStream, armor, withIntegrityCheck);
    }

    public static void EncryptFile(string inputFilePath, Stream outputStream, Stream publicKeyStream, bool armor, bool withIntegrityCheck)
    {
        var publicKey = ReadPublicKey(publicKeyStream);

        if (armor)
        {
            outputStream = new ArmoredOutputStream(outputStream);
        }

        // Initialize the encrypted data generator
        var encryptedData = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256, withIntegrityCheck, new SecureRandom());
        encryptedData.AddMethod(publicKey);

        try
        {
            // Open the encrypted output stream
            using var encryptedOut = encryptedData.Open(outputStream, new byte[1 << 16]);

            // Initialize the compressed data generator
            var compressedData = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
            try
            {
                // Open the compressed output stream
                using var compressedOut = compressedData.Open(encryptedOut);

                var literalData = new PgpLiteralDataGenerator();
                using var fileStream = File.OpenRead(inputFilePath);
                using var lOut = literalData.Open(compressedOut, PgpLiteralData.Binary, new FileInfo(inputFilePath).Name, fileStream.Length, DateTime.UtcNow);

                // Copy the file content to the literal data generator
                fileStream.CopyTo(lOut);
            }
            finally
            {
                compressedData.Close(); // Explicitly close the compressed data generator
            }
        }
        finally
        {
            encryptedData.Close(); // Explicitly close the encrypted data generator
        }

        if (armor)
        {
            outputStream.Close(); // Ensure the armored output stream is properly closed
        }
    }

    private static PgpPublicKey ReadPublicKey(Stream inputStream)
    {
        var pgpPub = new PgpPublicKeyRingBundle(PgpUtilities.GetDecoderStream(inputStream));
        foreach (PgpPublicKeyRing kRing in pgpPub.GetKeyRings())
        {
            foreach (PgpPublicKey k in kRing.GetPublicKeys())
            {
                if (k.IsEncryptionKey)
                {
                    return k;
                }
            }
        }
        throw new ArgumentException("No encryption key found in public key ring.");
    }
}
