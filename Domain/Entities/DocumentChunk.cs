using Domain.Entities.Abstracts;
using Domain.ValueObjects;
using Flunt.Validations;

namespace Domain.Entities;

internal class DocumentChunk : Entity
{
    private DocumentChunk() { }

    public Guid DocumentId { get; private set; }
    public ChunkText Text { get; private set; } = null!;
    public Embedding Embedding { get; private set; } = null!;
    public int ChunkIndex { get; private set; }

    // Navigation property (set by EF Core)
    public Document Document { get; private set; } = null!;

    public static DocumentChunk Create(Guid documentId, string text, float[] embedding, int chunkIndex)
    {
        var chunk = new DocumentChunk();

        var textVo = new ChunkText(text);
        var embeddingVo = new Embedding(embedding);

        chunk.AddNotificationsFromValueObjects(textVo, embeddingVo);

        chunk.AddNotifications(new Contract<DocumentChunk>()
            .Requires()
            .IsGreaterOrEqualsThan(chunkIndex, 0, nameof(ChunkIndex), "ChunkIndex deve ser >= 0"));

        if (chunk.IsValid)
        {
            chunk.DocumentId = documentId;
            chunk.Text = textVo;
            chunk.Embedding = embeddingVo;
            chunk.ChunkIndex = chunkIndex;
        }

        return chunk;
    }
}
