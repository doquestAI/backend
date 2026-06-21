namespace Domain.Configurations;

public class PubSubSettings
{
    public string ProjectId { get; set; } = string.Empty;
    public string CredentialsPath { get; set; } = string.Empty;
    public TopicSettings Topics { get; set; } = new();
    public SubscriptionSettings Subscriptions { get; set; } = new();
}

public class TopicSettings
{
    public string FileUpload { get; set; } = "file-upload-topic";
    public string FileDelete { get; set; } = "file-delete-topic";
    public string EmailNotification { get; set; } = "email-notification-topic";
    public string EmbeddingProcessing { get; set; } = "embedding-processing-topic";
    public string EmbeddingDeletionRequest { get; set; } = "embedding-deletion-request-topic";
    public string StorageDelete { get; set; } = "storage-delete-topic";
}

public class SubscriptionSettings
{
    public string FileUpload { get; set; } = "file-upload-subscription";
    public string FileDelete { get; set; } = "file-delete-subscription";
    public string EmailNotification { get; set; } = "email-notification-subscription";
    public string EmbeddingCompleted { get; set; } = "embedding-completed-subscription";
    public string EmbeddingDeletionCompleted { get; set; } = "embedding-deletion-completed-subscription";
    public string StorageDeleteCompleted { get; set; } = "storage-delete-completed-subscription";
    public int MaxConcurrentMessages { get; set; } = 10;
    public int AckDeadlineSeconds { get; set; } = 60;
}