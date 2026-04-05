namespace DoQuest.Application.UseCases.Documents.Commands.IngestDocument;

public sealed record IngestDocumentResponse(
    Guid DocumentId,
    string Title,
    int ChunksIngested);
