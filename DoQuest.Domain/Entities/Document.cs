using Flunt.Notifications;
using Flunt.Validations;

namespace DoQuest.Domain.Entities;

public class Document : Notifiable<Notification>
{
    private Document() { }

    private readonly List<DocumentChunk> _chunks = [];

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string SourceUrl { get; private set; } = string.Empty;
    public Guid VestibularId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<DocumentChunk> Chunks => _chunks.AsReadOnly();

    // Navigation property (set by EF Core)
    public Vestibular Vestibular { get; private set; } = null!;

    public static Document Create(string title, string sourceUrl, Guid vestibularId)
    {
        var document = new Document();

        document.AddNotifications(new Contract<Document>()
            .Requires()
            .IsNotNullOrEmpty(title, nameof(Title), "Título do documento é obrigatório")
            .IsNotNullOrEmpty(sourceUrl, nameof(SourceUrl), "URL de origem é obrigatória")
            .AreNotEquals(vestibularId, Guid.Empty, nameof(VestibularId), "VestibularId inválido"));

        if (document.IsValid)
        {
            document.Id = Guid.NewGuid();
            document.Title = title;
            document.SourceUrl = sourceUrl;
            document.VestibularId = vestibularId;
            document.CreatedAt = DateTime.UtcNow;
        }

        return document;
    }

    /// <summary>
    /// Adds a chunk to this document. Only valid chunks (text + embedding) are added.
    /// Propagates chunk validation errors to the document.
    /// </summary>
    public void AddChunk(string text, float[] embedding)
    {
        if (!IsValid) return;

        var chunk = DocumentChunk.Create(Id, text, embedding, _chunks.Count);

        if (!chunk.IsValid)
        {
            AddNotifications(chunk);
            return;
        }

        _chunks.Add(chunk);
    }
}
