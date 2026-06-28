namespace Domain.Configurations;

internal sealed class AppSettings
{
    public string BackendUrl { get; set; } = string.Empty;
    public string FrontendUrl { get; set; } = string.Empty;
    public string VersionApi { get; set; } = string.Empty;
    public string EncryptionKey { get; set; } = string.Empty;
    public string PublicUrlFrontEnd { get; set; } = string.Empty;
    public string CorsPolicyName { get; set; } = "DoQuestCorsPolicy";
    public string TempFilesPath { get; set; } = Path.Combine(Path.GetTempPath(), "documents_students");
}