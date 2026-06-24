using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Domain.Configurations;
using Domain.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Azure.BlobStorage;

internal sealed class AzureBlobStorageService : ICloudStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureBlobStorageService(IOptions<AzureSettings> settings)
    {
        _blobServiceClient = new BlobServiceClient(settings.Value.ConnectionString);
    }

    public async Task<string> UploadFileAsync(
        string containerName,
        string objectPath,
        Stream fileStream,
        string contentType,
        CancellationToken cancellationToken)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        var blobClient = containerClient.GetBlobClient(objectPath);

        await blobClient.UploadAsync(fileStream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType },
            Metadata = new Dictionary<string, string>
            {
                ["confidentiality-level"] = "high",
                ["uploaded-at"] = DateTime.UtcNow.ToString("O"),
                ["requires-backend-auth"] = "true"
            }
        }, cancellationToken);

        return objectPath;
    }

    public async Task<bool> DeleteFileAsync(
        string containerName,
        string objectPath,
        CancellationToken cancellationToken)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(objectPath);
        var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        return response.Value;
    }

    public async Task<Stream> DownloadFileAsync(
        string containerName,
        string objectPath,
        CancellationToken cancellationToken)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(objectPath);
        var download = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
        return download.Value.Content;
    }

    public async Task<bool> FileExistsAsync(
        string containerName,
        string objectPath,
        CancellationToken cancellationToken)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(objectPath);
        return await blobClient.ExistsAsync(cancellationToken);
    }
}
