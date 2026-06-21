using Domain.Interfaces.Handlers;
using Domain.Interfaces.Repositories;
using Domain.Messages;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Commands.Embeddings;

internal sealed partial class EmbeddingDeletionCompletedHandler(
    IDocumentRepository documentRepository,
    IDbCommit dbCommit,
    ILogger<EmbeddingDeletionCompletedHandler> logger) : IEmbeddingDeletionCompletedHandler
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
            await dbCommit.Commit(cancellationToken);

            LogDocumentDocumentidPermanentlyDeletedEmbeddingsDeletedEmbeddingsdeleted(logger, message.DocumentId, message.EmbeddingsDeleted);
        }
        else
        {
            document.MarkDeletionFailed();
            await dbCommit.Commit(cancellationToken);

            LogDocumentDocumentidDeletionFailedError(logger, message.DocumentId, message.ErrorMessage);
        }
    }

    [LoggerMessage(LogLevel.Information, "Processing embedding deletion completed for document {documentId}, Success: {Success}")]
    static partial void LogProcessingEmbeddingDeletionCompletedForDocumentDocumentidSuccessSuccess(ILogger<EmbeddingDeletionCompletedHandler> logger, Guid documentId, bool Success);

    [LoggerMessage(LogLevel.Warning, "Document {documentId} not found")]
    static partial void LogDocumentDocumentidNotFound(ILogger<EmbeddingDeletionCompletedHandler> logger, Guid documentId);

    [LoggerMessage(LogLevel.Information, "Document {documentId} permanently deleted. Embeddings deleted: {embeddingsDeleted}")]
    static partial void LogDocumentDocumentidPermanentlyDeletedEmbeddingsDeletedEmbeddingsdeleted(ILogger<EmbeddingDeletionCompletedHandler> logger, Guid documentId, int embeddingsDeleted);

    [LoggerMessage(LogLevel.Warning, "Document {documentId} deletion failed: {error}")]
    static partial void LogDocumentDocumentidDeletionFailedError(ILogger<EmbeddingDeletionCompletedHandler> logger, Guid documentId, string error);
}