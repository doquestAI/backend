using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Queries.Document.List;

internal class Handler(IDocumentRepository documentRepository) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var documents = await documentRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        var summaries = documents
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new DocumentSummary(
                d.Id,
                d.FileName?.Body?.ToString(),
                d.FileSizeBytes,
                d.Status.ToString(),
                d.ChunksGenerated))
            .ToList();

        return new Response(
            StatusCode: 200,
            Documents: summaries,
            TotalCount: documents.Count());
    }
}
