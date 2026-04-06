using Application.Common;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Documents.Queries.GetDocumentsByVestibular;

internal sealed class GetDocumentsByVestibularQueryHandler(IDocumentRepository documentRepository)
    : IRequestHandler<GetDocumentsByVestibularQuery, Result<IReadOnlyList<DocumentSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<DocumentSummaryDto>>> Handle(
        GetDocumentsByVestibularQuery request,
        CancellationToken cancellationToken)
    {
        var documents = await documentRepository.GetByVestibularIdAsync(request.VestibularId, cancellationToken);

        var dtos = documents
            .Select(d => new DocumentSummaryDto(d.Id, d.Title.Value, d.SourceUrl.Value, d.Chunks.Count, d.CreatedAt))
            .ToList()
            .AsReadOnly();

        return Result.Success<IReadOnlyList<DocumentSummaryDto>>(dtos);
    }
}
