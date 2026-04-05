using DoQuest.Application.Common;
using MediatR;

namespace DoQuest.Application.UseCases.Documents.Queries.GetDocumentsByVestibular;

public sealed record DocumentSummaryDto(Guid Id, string Title, string SourceUrl, int ChunkCount, DateTime CreatedAt);

public sealed record GetDocumentsByVestibularQuery(Guid VestibularId)
    : IRequest<Result<IReadOnlyList<DocumentSummaryDto>>>;
