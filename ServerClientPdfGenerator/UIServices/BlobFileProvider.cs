using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace ServerClientPdfGenerator.UIServices;

    public interface IStorageProvider
    {
        Task<BlobDto> FileInfoAsync(string blobFilename, string containerName, CancellationToken cancellationToken);
        Task<byte[]> DownloadAsync(string blobFilename, string containerName, CancellationToken cancellationToken);
        Task DeleteFileAsync(string fileName, string containerName, CancellationToken cancellationToken);
    }

    public class BlobDto
    {
        public string? Url { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public string? Content { get; set; }
        
    }

    public class BlobStorageProvider(ILogger<BlobStorageProvider> logger, IConfiguration configuration)
        : IStorageProvider
    {
        private readonly string? _storageConnectionString = configuration.GetValue<string>("ConnectionStrings:AzureStorage");
        private readonly string? _storageAccountName = configuration.GetValue<string>("AzureStorage:accountName");
        private readonly string? _storageAccountKey = configuration.GetValue<string>("AzureStorage:primaryKey");


        public Task<BlobDto> FileInfoAsync(string blobFilename, string containerName, CancellationToken cancellationToken)
        {
            var client = new BlobContainerClient(_storageConnectionString, containerName);

            var blobSasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = containerName,
                BlobName = blobFilename,
                ExpiresOn = DateTime.UtcNow.AddMinutes(60),
            };

            blobSasBuilder.SetPermissions(BlobSasPermissions.Read);
            var sasToken = blobSasBuilder
                .ToSasQueryParameters(new StorageSharedKeyCredential(_storageAccountName, _storageAccountKey)).ToString();


            var file = client.GetBlobClient(blobFilename);
            var result = new BlobDto()
            {
                Url = $"{file.Uri.AbsoluteUri}?{sasToken}",
                Name = file.Name
            };

            return Task.FromResult(result);
        }

        public async Task<byte[]> DownloadAsync(string blobFilename, string containerName,
            CancellationToken cancellationToken)
        {
            var client = new BlobContainerClient(_storageConnectionString, containerName);

            try
            {
                var file = client.GetBlobClient(blobFilename);


                if (await file.ExistsAsync(cancellationToken))
                {
                    var stream = new MemoryStream();
                    await file.DownloadToAsync(stream, cancellationToken);
                    stream.Seek(0, SeekOrigin.Begin);
                    byte[] bytes = stream.ToArray();
                    return bytes;
                }
            }
            catch (RequestFailedException ex)
                when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
            {
                logger.LogError($"File {blobFilename} was not found.");
                throw new FileNotFoundException($"Blob file not found {blobFilename}");
            }

            return null;
        }

        public async Task DeleteFileAsync(string fileName, string containerName, CancellationToken cancellationToken)
        {
            var client = new BlobContainerClient(_storageConnectionString,containerName);
            var blob = client.GetBlobClient(fileName);
            await blob.DeleteAsync(cancellationToken: cancellationToken);
        }
    }
    