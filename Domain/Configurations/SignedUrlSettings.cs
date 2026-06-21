namespace Domain.Configurations;

public class SignedUrlSettings
{
    public int IntervalMinutes { get; set; } = 5;
    public int BatchSize { get; set; } = 100;
    public int UrlExpirationDays { get; set; } = 7;
}