using Domain.Interfaces.Repositories;
using MediatR;
using DocumentEntity = Domain.Entities.Core.Document;

namespace Application.UseCases.Queries.Document.GetById;

internal class Handler(IDocumentRepository documentRepository) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var document = await documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);

        if (document is null)
            return new Response(StatusCode: 404, Message: "Document not found");

        return new Response(
            StatusCode: 200,
            DocumentId: document.Id,
            FileName: document.FileName?.Body?.ToString(),
            FileSizeBytes: document.FileSizeBytes,
            Status: document.Status.ToString(),
            ChunksGenerated: document.ChunksGenerated);
    }
}
