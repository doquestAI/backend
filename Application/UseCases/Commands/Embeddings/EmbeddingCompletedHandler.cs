using Domain.Interfaces.Handlers;
using Domain.Interfaces.Repositories;
using Domain.Messages;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Commands.Embeddings;

internal sealed partial class EmbeddingCompletedHandler(
    IDocumentRepository documentRepository,
    IDbCommit dbCommit,
    ILogger<EmbeddingCompletedHandler> logger) : IEmbeddingCompletedHandler
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

        await dbCommit.Commit(cancellationToken);
    }

    [LoggerMessage(LogLevel.Information, "Processing embedding completed for document {documentId}, Success: {success}")]
    static partial void LogProcessingEmbeddingCompletedForDocumentDocumentidSuccessSuccess(ILogger<EmbeddingCompletedHandler> logger, Guid documentId, bool success);

    [LoggerMessage(LogLevel.Warning, "Document {documentId} not found")]
    static partial void LogDocumentDocumentidNotFound(ILogger<EmbeddingCompletedHandler> logger, Guid documentId);

    [LoggerMessage(LogLevel.Information, "Document {documentId} marked as embedded. Chunks: {chunks}, Model: {model}")]
    static partial void LogDocumentDocumentidMarkedAsEmbeddedChunksChunksModelModel(ILogger<EmbeddingCompletedHandler> logger, Guid documentId, int chunks, string model);

    [LoggerMessage(LogLevel.Warning, "Document {documentId} embedding failed: {error}")]
    static partial void LogDocumentDocumentidEmbeddingFailedError(ILogger<EmbeddingCompletedHandler> logger, Guid documentId, string error);
}