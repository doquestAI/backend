using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services.AI.Embeddings;
using Domain.Messages;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.AI.Embeddings;

internal sealed partial class EmbeddingCompletedService(
    IDocumentRepository documentRepository,
    IUnitOfWork unitOfWork,
    ILogger<EmbeddingCompletedService> logger)
    : IEmbeddingCompletedService
{
    public async Task ExecuteAsync(EmbeddingCompletedMessage message, CancellationToken cancellationToken)
    {
        LogProcessingEmbeddingCompletedForDocumentDocumentidSuccessSuccess(logger, message.DocumentId, message.Success);
        var document = await documentRepository.GetByIdAsync(message.DocumentId, cancellationToken);
        if (document == null)
        {
            LogDocumentDocumentidNotFound(logger, message.DocumentId);
            return;
        }

        if (message.Success)
        {
            document.MarkEmbeddingCompleted(message.ChunksGenerated, message.EmbeddingModel);
            LogDocumentDocumentidMarkedAsEmbeddedChunksChunksModelModel(logger, message.DocumentId, message.ChunksGenerated, message.EmbeddingModel);
        }
        else
        {
            document.MarkEmbeddingFailed(message.ErrorMessage ?? "Unknown error");
            if (message.ErrorMessage != null)
            {
                LogDocumentDocumentidEmbeddingFailedError(logger,
                    message.DocumentId, message.ErrorMessage);
            }
        }

        await unitOfWork.CommitAsync(cancellationToken);
    }

    [LoggerMessage(LogLevel.Information, "Processing embedding completed for document {documentId}, Success: {success}")]
    static partial void LogProcessingEmbeddingCompletedForDocumentDocumentidSuccessSuccess(ILogger<EmbeddingCompletedService> logger, Guid documentId, bool success);

    [LoggerMessage(LogLevel.Warning, "Document {documentId} not found")]
    static partial void LogDocumentDocumentidNotFound(ILogger<EmbeddingCompletedService> logger, Guid documentId);

    [LoggerMessage(LogLevel.Information, "Document {documentId} marked as embedded. Chunks: {chunks}, Model: {model}")]
    static partial void LogDocumentDocumentidMarkedAsEmbeddedChunksChunksModelModel(ILogger<EmbeddingCompletedService> logger, Guid documentId, int chunks, string model);

    [LoggerMessage(LogLevel.Warning, "Document {documentId} embedding failed: {error}")]
    static partial void LogDocumentDocumentidEmbeddingFailedError(ILogger<EmbeddingCompletedService> logger, Guid documentId, string error);
}