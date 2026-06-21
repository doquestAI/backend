namespace Domain.Messages;

public record EmbeddingDeletionCompletedMessage(
    Guid DocumentId,
    DateTime DeletedAt,
    int EmbeddingsDeleted,
    bool Success,
    string? ErrorMessage);