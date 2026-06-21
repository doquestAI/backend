namespace Domain.Messages;

public record EmbeddingCompletedMessage(
    Guid DocumentId,
    bool Success,
    int ChunksGenerated,
    string EmbeddingModel,
    DateTime ProcessedAt,
    string? ErrorMessage,
    EmbeddingMetadata? Metadata);

public record EmbeddingMetadata(
    int TotalTokens,
    long ProcessingTimeMs);