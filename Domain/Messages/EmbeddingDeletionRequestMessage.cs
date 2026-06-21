namespace Domain.Messages;

public record EmbeddingDeletionRequestMessage(
    Guid DocumentId,
    DateTime RequestedAt,
    Guid RequestedBy);