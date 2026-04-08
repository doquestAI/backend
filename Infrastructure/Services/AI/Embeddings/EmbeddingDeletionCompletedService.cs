using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services.AI.Embeddings;
using Domain.Messages;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.AI.Embeddings;

internal sealed partial class EmbeddingDeletionCompletedService(
    IDocumentRepository documentRepository,
    IUnitOfWork unitOfWork,
    ILogger<EmbeddingDeletionCompletedService> logger) : IEmbeddingDeletionCompletedService
{
    public async Task ExecuteAsync(EmbeddingDeletionCompletedMessage message, CancellationToken cancellationToken)
    {
        LogProcessingEmbeddingDeletionCompletedForDocumentDocumentidSuccessSuccess(logger, message.DocumentId, message.Success);

        var document = await documentRepository.GetByIdAsync(message.DocumentId, cancellationToken);
        if (document == null)
        {
            LogDocumentDocumentidNotFound(logger, message.DocumentId);
            return;
        }

        if (message.Success)
        {
            documentRepository.Delete(document);
            await unitOfWork.CommitAsync(cancellationToken);

            LogDocumentDocumentidPermanentlyDeletedEmbeddingsDeletedEmbeddingsdeleted(logger, message.DocumentId, message.EmbeddingsDeleted);
        }
        else
        {
            document.MarkDeletionFailed();
            await unitOfWork.CommitAsync(cancellationToken);

            LogDocumentDocumentidDeletionFailedError(logger, message.DocumentId, message.ErrorMessage);
        }
    }

    [LoggerMessage(LogLevel.Information, "Processing embedding deletion completed for document {documentId}, Success: {Success}")]
    static partial void LogProcessingEmbeddingDeletionCompletedForDocumentDocumentidSuccessSuccess(ILogger<EmbeddingDeletionCompletedService> logger, Guid documentId, bool Success);

    [LoggerMessage(LogLevel.Warning, "Document {documentId} not found")]
    static partial void LogDocumentDocumentidNotFound(ILogger<EmbeddingDeletionCompletedService> logger, Guid documentId);

    [LoggerMessage(LogLevel.Information, "Document {documentId} permanently deleted. Embeddings deleted: {embeddingsDeleted}")]
    static partial void LogDocumentDocumentidPermanentlyDeletedEmbeddingsDeletedEmbeddingsdeleted(ILogger<EmbeddingDeletionCompletedService> logger, Guid documentId, int embeddingsDeleted);

    [LoggerMessage(LogLevel.Warning, "Document {documentId} deletion failed: {error}")]
    static partial void LogDocumentDocumentidDeletionFailedError(ILogger<EmbeddingDeletionCompletedService> logger, Guid documentId, string error);
}