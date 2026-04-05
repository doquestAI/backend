using DoQuest.Application.Abstractions;
using DoQuest.Application.Common;
using DoQuest.Domain.Entities;
using DoQuest.Domain.Repositories;
using MediatR;

namespace DoQuest.Application.UseCases.Documents.Commands.IngestDocument;

public sealed class IngestDocumentCommandHandler(
    IDocumentRepository documentRepository,
    IEmbeddingService embeddingService,
    IDocumentChunker chunker,
    IUnitOfWork unitOfWork)
    : IRequestHandler<IngestDocumentCommand, Result<IngestDocumentResponse>>
{
    public async Task<Result<IngestDocumentResponse>> Handle(
        IngestDocumentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Create document aggregate root
        var document = Document.Create(request.Title, request.SourceUrl, request.VestibularId);
        if (document.IsInvalid)
            return Result.Failure<IngestDocumentResponse>(document.Notifications);

        // 2. Chunk the raw text
        var textChunks = chunker.Chunk(request.RawText);
        if (textChunks.Count == 0)
            return Result.Failure<IngestDocumentResponse>("RawText", "Nenhum chunk gerado. Verifique se o texto não está vazio.");

        // 3. Generate embeddings and add chunks to aggregate
        foreach (var chunkText in textChunks)
        {
            var embedding = await embeddingService.GenerateAsync(chunkText, cancellationToken);
            document.AddChunk(chunkText, embedding);

            // Stop on first invalid chunk — propagate error
            if (!document.IsValid)
                return Result.Failure<IngestDocumentResponse>(document.Notifications);
        }

        // 4. Persist
        await documentRepository.AddAsync(document, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new IngestDocumentResponse(document.Id, document.Title, document.Chunks.Count));
    }
}
