using Domain.Entities.Abstracts;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities.Core;

internal class Document : Archive
{
    public BigString FileName { get; private set; } = null!;
    public long FileSizeBytes { get; private set; }
    public DocumentStatus Status { get; private set; }
    public Guid UploadedByUserId { get; private set; }
    public bool IsEmbedding { get; private set; }
    public EmbeddingStatus? EmbeddingProcessingStatus { get; private set; }
    public int? ChunksGenerated { get; private set; }
    public DateTime? EmbeddingCompletedAt { get; private set; }
    public string? EmbeddingErrorMessage { get; private set; }
    public string? EmbeddingModel { get; private set; }
    public DateTime? DeletionRequestedAt { get; private set; }
    public Guid? DeletionRequestedBy { get; private set; }
    public string? Metadata { get; private set; }

    private Document() { }

    public Document(
        BigString fileName,
        string containerName,
        ContentType contentType,
        long fileSizeBytes,
        Guid uploadedByUserId,
        BigString? localTemporaryPath = null,
        string? metadata = null)
    {
        AddNotificationsFromValueObjects(fileName, localTemporaryPath);
        FileName = fileName;
        ContainerName = containerName;
        ContentType = contentType;
        FileSizeBytes = fileSizeBytes;
        UploadedByUserId = uploadedByUserId;
        LocalTemporaryPath = localTemporaryPath;
        Metadata = metadata;
        Status = DocumentStatus.Pending;
        IsEmbedding = false;
    }

    public void MarkAsUploading()
    {
        if (Status != DocumentStatus.Pending)
        {
            AddNotification("Status", "Document must be Pending to start uploading");
            return;
        }
        Status = DocumentStatus.Uploading;
    }

    public void SetCloudStorageKey(BigString cloudStorageKey)
    {
        AddNotificationsFromValueObjects(cloudStorageKey);
        CloudStorageKey = cloudStorageKey;
        Status = DocumentStatus.Ready;
    }

    public void SetSignedUrl(Url url, DateTime expiration)
    {
        AddNotificationsFromValueObjects(url);
        SignedUrl = url;
        SignedUrlExpiration = expiration;
    }

    public void ClearSignedUrl()
    {
        SignedUrl = null;
        SignedUrlExpiration = null;
    }

    public void MarkAsProcessing()
    {
        if (Status != DocumentStatus.Ready)
        {
            AddNotification("Status", "Document must be Ready to start processing");
            return;
        }
        Status = DocumentStatus.Processing;
        EmbeddingProcessingStatus = EmbeddingStatus.Processing;
    }

    public void MarkEmbeddingCompleted(int chunksGenerated, string embeddingModel)
    {
        Status = DocumentStatus.Embedded;
        IsEmbedding = true;
        EmbeddingProcessingStatus = EmbeddingStatus.Completed;
        ChunksGenerated = chunksGenerated;
        EmbeddingModel = embeddingModel;
        EmbeddingCompletedAt = DateTime.UtcNow;
        EmbeddingErrorMessage = null;
        IsActive = true;
    }

    public void MarkEmbeddingFailed(string errorMessage)
    {
        EmbeddingProcessingStatus = EmbeddingStatus.Failed;
        EmbeddingErrorMessage = errorMessage;
        Status = DocumentStatus.Failed;
    }

    public void MarkAsFailed()
    {
        Status = DocumentStatus.Failed;
    }

    public void RequestDeletion(Guid requestedBy)
    {
        Status = DocumentStatus.DeletionPending;
        DeletionRequestedAt = DateTime.UtcNow;
        DeletionRequestedBy = requestedBy;
    }

    public void MarkDeletionFailed()
    {
        Status = DocumentStatus.DeletionFailed;
    }

    public void ClearLocalPath()
    {
        LocalTemporaryPath = null;
    }

    public bool HasValidSignedUrl()
    {
        return SignedUrl != null &&
               SignedUrlExpiration.HasValue &&
               SignedUrlExpiration.Value > DateTime.UtcNow;
    }

    public bool NeedsSignedUrl()
    {
        return CloudStorageKey != null &&
               (SignedUrl == null || !SignedUrlExpiration.HasValue || SignedUrlExpiration.Value <= DateTime.UtcNow);
    }

    public void UpdateLocalPath(BigString localPath) => LocalTemporaryPath = localPath;
}