using Application.Common;
using MediatR;

namespace Application.UseCases.Documents.Queries.GetDocumentsByVestibular;

internal sealed record DocumentSummaryDto(Guid Id, string Title, string SourceUrl,
     int ChunkCount, DateTime CreatedAt);

internal sealed record GetDocumentsByVestibularQuery(Guid VestibularId)
    : IRequest<Result<IReadOnlyList<DocumentSummaryDto>>>;