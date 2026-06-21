namespace Domain.Configurations;

public class AzureSettings
{
    public string ProjectId { get; set; } = string.Empty;
    public string CredentialsPath { get; set; } = string.Empty;
    public string BucketDocuments { get; set; } = string.Empty;
    public string BucketMedia { get; set; } = string.Empty;
    public int SignedUrlDurationHoursDocuments { get; set; } = 1;
    public int SignedUrlDurationHoursMedia { get; set; } = 1;
}