namespace Domain.Messages;

internal record StorageUploadMessage(
    Guid DocumentId,
    string ContainerName,
    string ObjectPath,
    string ContentType,
    string? LocalFilePath);