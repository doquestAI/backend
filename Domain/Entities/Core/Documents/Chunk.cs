using Domain.Entities.Abstracts;

namespace Domain.Entities.Core.Documents;

internal class Chunk : Entity
{
    public Guid DocumentId { get; private set; }
    public int PositionIndex { get; private set; }
    public string Content { get; private set; } = null!;
    public string? Metadata { get; private set; }
    public byte[]? Embedding { get; internal set; }

    private Chunk() { }

    public Chunk(Guid documentId, int positionIndex, string content, string? metadata = null)
    {
        DocumentId = documentId;
        PositionIndex = positionIndex;
        Content = content;
        Metadata = metadata;
    }

    public void SetEmbedding(ReadOnlyMemory<float> embedding)
    {
        Embedding = System.Runtime.InteropServices.MemoryMarshal.AsBytes<float>(embedding.Span).ToArray();
    }

    public ReadOnlyMemory<float> GetEmbedding()
    {
        if (Embedding == null) return ReadOnlyMemory<float>.Empty;
        var floats = System.Runtime.InteropServices.MemoryMarshal.Cast<byte, float>(new ReadOnlyMemory<byte>(Embedding).Span).ToArray();
        return new ReadOnlyMemory<float>(floats);
    }

    public void MarkAsDeleted()
    {
        SetValuesDelete();
    }
}
