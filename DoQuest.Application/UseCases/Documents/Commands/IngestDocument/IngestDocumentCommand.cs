using DoQuest.Application.Common;
using MediatR;

namespace DoQuest.Application.UseCases.Documents.Commands.IngestDocument;

public sealed record IngestDocumentCommand(
    string Title,
    string SourceUrl,
    Guid VestibularId,
    string RawText
) : IRequest<Result<IngestDocumentResponse>>;
