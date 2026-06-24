using Domain.Configurations;
using Domain.Interfaces.Handlers;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Messages;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.UseCases.Commands.Storage;

internal sealed partial class StorageUploadHandler(
    IDocumentRepository documentRepository,
    IDbCommit dbCommit,
    ICloudStorageService cloudStorageService,
    IMessagePublisher messagePublisher,
    IOptions<ServiceBusSettings> pubSubSettings,
    ILogger<StorageUploadHandler> logger) : IStorageUploadHandler
{
    public async Task ExecuteAsync(StorageUploadMessage message, CancellationToken cancellationToken)
    {
        LogProcessingUploadForDocumentDocumentid(logger, message.DocumentId);

        var document = await documentRepository.GetByIdAsync(message.DocumentId, cancellationToken);
        if (document == null)
        {
            LogDocumentDocumentidNotFound(logger, message.DocumentId);
            return;
        }

        document.MarkAsUploading();
        await dbCommit.Commit(cancellationToken);

        try
        {
            if (string.IsNullOrEmpty(message.LocalFilePath) || !File.Exists(message.LocalFilePath))
            {
                if (message.LocalFilePath != null)
                {
                    LogLocalFileNotFoundLocalfilepath(logger,
                        message.LocalFilePath);
                }

                document.MarkAsFailed();
                await dbCommit.Commit(cancellationToken);
                return;
            }

            {
                await using var fileStream = File.OpenRead(message.LocalFilePath);
                await cloudStorageService.UploadFileAsync(
                    message.ContainerName,
                    message.ObjectPath,
                    fileStream,
                    message.ContentType,
                    cancellationToken);
            }

            document.SetCloudStorageKey(new BigString(message.ObjectPath));
            await dbCommit.Commit(cancellationToken);

            LogUploadCompletedForDocumentDocumentid(logger, message.DocumentId);

            var embeddingMessage = new DocumentEmbeddingMessage(
                DocumentId: document.Id,
                CloudStorageKey: message.ObjectPath,
                ContainerName: message.ContainerName,
                ContentType: message.ContentType,
                FileName: document.FileName.Body!,
                FileSizeBytes: document.FileSizeBytes,
                UploadedByUserId: document.UploadedByUserId);

            await messagePublisher.PublishAsync(
                pubSubSettings.Value.Queues.EmbeddingProcessing,
                embeddingMessage,
                cancellationToken);

            LogPublishedEmbeddingMessageForDocumentDocumentid(logger, message.DocumentId);

            await DeleteTemporaryFileAsync(message.LocalFilePath, cancellationToken);
            document.ClearLocalPath();
            await dbCommit.Commit(cancellationToken);
        }
        catch (Exception ex)
        {
            LogErrorUploadingDocumentDocumentid(logger, ex, message.DocumentId);
            document.MarkAsFailed();
            await dbCommit.Commit(cancellationToken);
            throw;
        }
    }

    private async Task DeleteTemporaryFileAsync(string filePath, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return;

        const int MaxRetries = 5;
        const int BaseDelayMs = 100;

        for (var attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                if (attempt > 0)
                {
                    await Task.Delay(BaseDelayMs * attempt, cancellationToken);
                }

                File.Delete(filePath);
                LogTemporaryFileDeletedFilepath(logger, filePath);
                return;
            }
            catch (IOException ex) when (attempt < MaxRetries - 1)
            {
                LogAttemptAttemptMaxretriesToDeleteFileFailedFilepathErrorError(logger, attempt + 1, MaxRetries, filePath, ex.Message);
            }
            catch (IOException ex)
            {
                LogFailedToDeleteTemporaryFileAfterMaxretriesAttemptsFilepath(logger, ex, MaxRetries, filePath);
                return;
            }
        }
    }

    [LoggerMessage(LogLevel.Information, "Processing upload for document {documentId}")]
    static partial void LogProcessingUploadForDocumentDocumentid(ILogger<StorageUploadHandler> logger, Guid documentId);

    [LoggerMessage(LogLevel.Warning, "Document {documentId} not found")]
    static partial void LogDocumentDocumentidNotFound(ILogger<StorageUploadHandler> logger, Guid documentId);

    [LoggerMessage(LogLevel.Error, "Local file not found: {localFilePath}")]
    static partial void LogLocalFileNotFoundLocalfilepath(ILogger<StorageUploadHandler> logger, string localFilePath);

    [LoggerMessage(LogLevel.Information, "Upload completed for document {documentId}")]
    static partial void LogUploadCompletedForDocumentDocumentid(ILogger<StorageUploadHandler> logger, Guid documentId);

    [LoggerMessage(LogLevel.Information, "Published embedding message for document {documentId}")]
    static partial void LogPublishedEmbeddingMessageForDocumentDocumentid(ILogger<StorageUploadHandler> logger, Guid documentId);

    [LoggerMessage(LogLevel.Error, "Error uploading document {documentId}")]
    static partial void LogErrorUploadingDocumentDocumentid(
        ILogger<StorageUploadHandler> logger, Exception exception,
        Guid documentId);

    [LoggerMessage(LogLevel.Information, "Temporary file deleted: {filePath}")]
    static partial void LogTemporaryFileDeletedFilepath(ILogger<StorageUploadHandler> logger, string filePath);

    [LoggerMessage(LogLevel.Warning, "Attempt {attempt}/{maxRetries} to delete file failed: {filePath}. Error: {error}")]
    static partial void LogAttemptAttemptMaxretriesToDeleteFileFailedFilepathErrorError(ILogger<StorageUploadHandler> logger, int attempt, int maxRetries, string filePath, string error);

    [LoggerMessage(LogLevel.Error, "Failed to delete temporary file after {maxRetries} attempts: {filePath}")]
    static partial void
        LogFailedToDeleteTemporaryFileAfterMaxretriesAttemptsFilepath(
            ILogger<StorageUploadHandler> logger, IOException ioException,
            int maxRetries, string filePath);
}