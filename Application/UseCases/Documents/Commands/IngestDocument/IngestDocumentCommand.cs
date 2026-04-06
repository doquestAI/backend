using Application.Common;
using MediatR;

namespace Application.UseCases.Documents.Commands.IngestDocument;

internal sealed record IngestDocumentCommand(
    string Title,
    string SourceUrl,
    Guid VestibularId,
    string RawText
) : IRequest<Result<IngestDocumentResponse>>;
