using Domain.Configurations;
using Domain.Interfaces.Context;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Messages;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.UseCases.Commands.Document.Delete;

internal sealed class Handler(
    IDocumentRepository documentRepository,
    ICloudStorageService cloudStorageService,
    IMessagePublisher messagePublisher,
    IDbCommit dbCommit,
    IUserContext userContext,
    IOptions<PubSubSettings> pubSubSettings,
    ILogger<Handler> logger) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing delete request for document {DocumentId}", request.DocumentId);

        var document = await documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
        if (document == null)
        {
            logger.LogWarning("Document {DocumentId} not found", request.DocumentId);
            return new Response(
                StatusCode: 404,
                Message: "Document not found",
                Success: false);
        }

        if (document.UploadedByUserId != userContext.UserId)
        {
            logger.LogWarning(
                "User {UserId} attempted to delete document uploaded by {OwnerId}",
                userContext.UserId,
                document.UploadedByUserId);
            return new Response(
                StatusCode: 403,
                Message: "Document not found",
                Success: false);
        }

        try
        {
            if (document.CloudStorageKey?.Body != null)
            {
                await cloudStorageService.DeleteFileAsync(
                    document.ContainerName,
                    document.CloudStorageKey.Body,
                    cancellationToken);

                logger.LogInformation(
                    "Deleted file from GCS for document {DocumentId}: {Path}",
                    request.DocumentId,
                    document.CloudStorageKey.Body);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting file from GCS for document {DocumentId}", request.DocumentId);
        }

        document.RequestDeletion(userContext.UserId);
        await dbCommit.Commit(cancellationToken);

        if (document.IsEmbedding)
        {
            var deletionMessage = new EmbeddingDeletionRequestMessage(
                DocumentId: document.Id,
                RequestedAt: DateTime.UtcNow,
                RequestedBy: userContext.UserId);

            await messagePublisher.PublishAsync(
                pubSubSettings.Value.Topics.EmbeddingDeletionRequest,
                deletionMessage,
                cancellationToken);

            logger.LogInformation(
                "Published embedding deletion request for document {DocumentId}",
                request.DocumentId);

            return new Response(
                StatusCode: 202,
                Message: "Deletion initiated. Waiting for embedding removal confirmation.",
                Success: true);
        }

        documentRepository.Delete(document);
        await dbCommit.Commit(cancellationToken);

        logger.LogInformation("Document {DocumentId} permanently deleted (no embeddings)", request.DocumentId);
        return new Response(
            StatusCode: 200,
            Message: "Document deleted successfully",
            Success: true);
    }
}