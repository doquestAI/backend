using Domain.Messages;

namespace Domain.Interfaces.Handlers;

internal interface IStorageUploadHandler
{
    Task ExecuteAsync(StorageUploadMessage message, CancellationToken cancellationToken);
}