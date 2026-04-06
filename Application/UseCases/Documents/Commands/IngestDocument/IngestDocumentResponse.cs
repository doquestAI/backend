namespace Application.UseCases.Documents.Commands.IngestDocument;

internal sealed record IngestDocumentResponse(
    Guid DocumentId,
    string Title,
    int ChunksIngested);
