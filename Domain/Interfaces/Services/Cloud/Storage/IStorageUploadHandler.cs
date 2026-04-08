using Domain.Messages;

namespace Domain.Interfaces.Services.Cloud.Storage;

internal interface IStorageUploadService
{
    Task ExecuteAsync(StorageUploadMessage message, CancellationToken cancellationToken);
}