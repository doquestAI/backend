using Domain.Entities;

namespace Domain.Records;

internal sealed record SearchResult(
    DocumentChunk Chunk,
    float Score);