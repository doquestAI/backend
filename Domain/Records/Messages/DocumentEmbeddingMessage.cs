namespace Domain.Messages;

internal record DocumentEmbeddingMessage(
    Guid DocumentId,
    string CloudStorageKey,
    string ContainerName,
    string ContentType,
    string FileName,
    long FileSizeBytes,
    Guid UploadedByUserId);
