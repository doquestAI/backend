using Domain.Entities.Abstracts;
using Domain.ValueObjects;
using Flunt.Validations;

namespace Domain.Entities;

internal class Document : Entity
{
    private Document() { }

    private readonly List<DocumentChunk> _chunks = [];

    public DocumentTitle Title { get; private set; } = null!;
    public Url SourceUrl { get; private set; } = null!;
    public Guid VestibularId { get; private set; }
    public IReadOnlyCollection<DocumentChunk> Chunks => _chunks.AsReadOnly();

    // Navigation property (set by EF Core)
    public Vestibular Vestibular { get; private set; } = null!;

    public static Document Create(string title, string sourceUrl, Guid vestibularId)
    {
        var document = new Document();

        var titleVo = new DocumentTitle(title);
        var sourceUrlVo = new Url(sourceUrl);

        document.AddNotificationsFromValueObjects(titleVo, sourceUrlVo);

        document.AddNotifications(new Contract<Document>()
            .Requires()
            .AreNotEquals(vestibularId, Guid.Empty, nameof(VestibularId), "VestibularId inválido"));

        if (document.IsValid)
        {
            document.Title = titleVo;
            document.SourceUrl = sourceUrlVo;
            document.VestibularId = vestibularId;
        }

        return document;
    }

    public void AddChunk(string text, float[] embedding)
    {
        if (!IsValid)
            return;

        var chunk = DocumentChunk.Create(Id, text, embedding, _chunks.Count);

        if (!chunk.IsValid)
        {
            AddNotifications(chunk);
            return;
        }

        _chunks.Add(chunk);
    }
}