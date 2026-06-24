namespace Domain.Messages;

public record DocumentEmbeddingMessage(
    Guid DocumentId,
    string CloudStorageKey,
    string ContainerName,
    string ContentType,
    string FileName,
    long FileSizeBytes,
    Guid UploadedByUserId);