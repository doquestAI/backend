using DoQuest.Application.Common;
using DoQuest.Domain.Repositories;
using MediatR;

namespace DoQuest.Application.UseCases.Documents.Queries.GetDocumentsByVestibular;

public sealed class GetDocumentsByVestibularQueryHandler(IDocumentRepository documentRepository)
    : IRequestHandler<GetDocumentsByVestibularQuery, Result<IReadOnlyList<DocumentSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<DocumentSummaryDto>>> Handle(
        GetDocumentsByVestibularQuery request,
        CancellationToken cancellationToken)
    {
        var documents = await documentRepository.GetByVestibularIdAsync(request.VestibularId, cancellationToken);

        var dtos = documents
            .Select(d => new DocumentSummaryDto(d.Id, d.Title, d.SourceUrl, d.Chunks.Count, d.CreatedAt))
            .ToList()
            .AsReadOnly();

        return Result.Success<IReadOnlyList<DocumentSummaryDto>>(dtos);
    }
}
