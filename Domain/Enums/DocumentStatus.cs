namespace Domain.Enums;

public enum DocumentStatus
{
    Pending = 1,
    Uploading = 2,
    Ready = 3,
    Processing = 4,
    Embedded = 5,
    Failed = 6,
    DeletionPending = 7,
    DeletionFailed = 8
}

public enum EmbeddingStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4
}