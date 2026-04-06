namespace Domain.Messages;

internal record EmbeddingCompletedMessage(
    Guid DocumentId,
    bool Success,
    int ChunksGenerated,
    string EmbeddingModel,
    DateTime ProcessedAt,
    string? ErrorMessage,
    EmbeddingMetadata? Metadata);

internal record EmbeddingMetadata(
    int TotalTokens,
    long ProcessingTimeMs);