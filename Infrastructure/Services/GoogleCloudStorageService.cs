using Infrastructure.Configurations;
using Domain.Interfaces.Services;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

internal sealed class GoogleCloudStorageService : ICloudStorageService
{
    private readonly StorageClient _storageClient;

    public GoogleCloudStorageService(IOptions<GoogleCloudSettings> settings)
    {
        var settings1 = settings.Value;

        GoogleCredential credential;

        if (!string.IsNullOrEmpty(settings1.CredentialsPath) && File.Exists(settings1.CredentialsPath))
        {
            credential = GoogleCredential.FromFile(settings1.CredentialsPath);
        }
        else
        {
            credential = GoogleCredential.GetApplicationDefault();
        }

        _storageClient = StorageClient.Create(credential);
    }

    public async Task<string> UploadFileAsync(
       string containerName,
       string objectPath,
       Stream fileStream,
       string contentType,
       CancellationToken cancellationToken)
    {
        var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        var uploadedObject = await _storageClient.UploadObjectAsync(
            bucket: containerName,
            objectName: objectPath,
            contentType: contentType,
            source: memoryStream,
            cancellationToken: cancellationToken);

        uploadedObject.Metadata = new Dictionary<string, string>
        {
            { "confidentiality-level", "high" },
            { "uploaded-at", DateTime.UtcNow.ToString("O") },
            { "requires-backend-auth", "true" }
        };

        await _storageClient.UpdateObjectAsync(
            uploadedObject,
            cancellationToken: cancellationToken);

        return objectPath;
    }

    public async Task<bool> DeleteFileAsync(
        string containerName,
        string objectPath,
        CancellationToken cancellationToken)
    {
        try
        {
            await _storageClient.DeleteObjectAsync(
                containerName,
                objectPath,
                cancellationToken: cancellationToken);

            return true;
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<Stream> DownloadFileAsync(
        string containerName,
        string objectPath,
        CancellationToken cancellationToken)
    {
        var memoryStream = new MemoryStream();

        await _storageClient.DownloadObjectAsync(
            containerName,
            objectPath,
            memoryStream,
            cancellationToken: cancellationToken);

        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task<bool> FileExistsAsync(
        string containerName,
        string objectPath,
        CancellationToken cancellationToken)
    {
        try
        {
            await _storageClient.GetObjectAsync(
                containerName,
                objectPath,
                cancellationToken: cancellationToken);

            return true;
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}