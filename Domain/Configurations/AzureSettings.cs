namespace Domain.Configurations;

public class AzureSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerDocuments { get; set; } = string.Empty;
    public string ContainerMedia { get; set; } = string.Empty;
    public int SasUrlDurationHoursDocuments { get; set; } = 1;
    public int SasUrlDurationHoursMedia { get; set; } = 1;
}