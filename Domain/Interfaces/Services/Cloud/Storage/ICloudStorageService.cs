namespace Domain.Interfaces.Services.Cloud.Storage;

public interface ICloudStorageService
{
    Task<string> UploadFileAsync(
        string containerName,
        string objectPath,
        Stream fileStream,
        string contentType,
        CancellationToken cancellationToken);

    Task<bool> DeleteFileAsync(
        string containerName,
        string objectPath,
        CancellationToken cancellationToken);

    Task<Stream> DownloadFileAsync(
        string containerName,
        string objectPath,
        CancellationToken cancellationToken);

    Task<bool> FileExistsAsync(
        string containerName,
        string objectPath,
        CancellationToken cancellationToken);

}