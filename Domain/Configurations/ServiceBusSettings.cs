namespace Domain.Configurations;

public class ServiceBusSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public ServiceBusQueueSettings Queues { get; set; } = new();
}

public class ServiceBusQueueSettings
{
    public string FileUpload { get; set; } = "file-upload";
    public string FileDelete { get; set; } = "file-delete";
    public string EmailNotification { get; set; } = "email-notification";
    public string EmbeddingProcessing { get; set; } = "embedding-processing";
    public string EmbeddingDeletionRequest { get; set; } = "embedding-deletion-request";
    public string StorageDelete { get; set; } = "storage-delete";
    public string EmbeddingCompleted { get; set; } = "embedding-completed";
    public string EmbeddingDeletionCompleted { get; set; } = "embedding-deletion-completed";
    public string StorageDeleteCompleted { get; set; } = "storage-delete-completed";
    public int MaxConcurrentMessages { get; set; } = 10;
}
