using Flunt.Notifications;
using Flunt.Validations;
using DoQuest.Domain.Constants;

namespace DoQuest.Domain.Entities;

public class DocumentChunk : Notifiable<Notification>
{
    private DocumentChunk() { }

    public Guid Id { get; private set; }
    public Guid DocumentId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public float[] Embedding { get; private set; } = [];
    public int ChunkIndex { get; private set; }

    // Navigation property (set by EF Core)
    public Document Document { get; private set; } = null!;

    public static DocumentChunk Create(Guid documentId, string text, float[] embedding, int chunkIndex)
    {
        var chunk = new DocumentChunk();

        chunk.AddNotifications(new Contract<DocumentChunk>()
            .Requires()
            .IsNotNullOrEmpty(text, nameof(Text), "Texto do chunk é obrigatório")
            .IsGreaterOrEqualsThan(chunkIndex, 0, nameof(ChunkIndex), "ChunkIndex deve ser >= 0"));

        if (embedding == null || embedding.Length == 0)
            chunk.AddNotification(new Notification(nameof(Embedding), "Embedding é obrigatório"));
        else if (embedding.Length != ModelConstants.EmbeddingDimension)
            chunk.AddNotification(new Notification(nameof(Embedding),
                $"Embedding deve ter {ModelConstants.EmbeddingDimension} dimensões, mas recebeu {embedding.Length}"));

        if (chunk.IsValid)
        {
            chunk.Id = Guid.NewGuid();
            chunk.DocumentId = documentId;
            chunk.Text = text;
            chunk.Embedding = embedding!;
            chunk.ChunkIndex = chunkIndex;
        }

        return chunk;
    }
}
