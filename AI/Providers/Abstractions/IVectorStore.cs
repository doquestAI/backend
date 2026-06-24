namespace AI.Providers.Abstractions;

/// <summary>
/// Armazenamento vetorial para RAG.
/// Implementação pode ser Qdrant, Pinecone, Azure AI Search, etc.
/// </summary>
public interface IVectorStore
{
    Task UpsertAsync(
        string collection,
        string id,
        string content,
        ReadOnlyMemory<float> embedding,
        IDictionary<string, string>? metadata = null,
        CancellationToken ct = default);

    Task<IReadOnlyList<VectorSearchResult>> SearchAsync(
        string collection,
        ReadOnlyMemory<float> queryEmbedding,
        int topK = 5,
        float threshold = 0.75f,
        CancellationToken ct = default);
}

public sealed record VectorSearchResult(
    string Id,
    string Content,
    float Score,
    IReadOnlyDictionary<string, string> Metadata
);