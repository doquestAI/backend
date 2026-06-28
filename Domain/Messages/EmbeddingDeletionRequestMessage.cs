namespace Domain.Messages;

internal record EmbeddingDeletionRequestMessage(
    Guid DocumentId,
    DateTime RequestedAt,
    Guid RequestedBy);