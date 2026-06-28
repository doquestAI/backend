namespace Domain.Messages;

internal record EmbeddingDeletionCompletedMessage(
    Guid DocumentId,
    DateTime DeletedAt,
    int EmbeddingsDeleted,
    bool Success,
    string? ErrorMessage);