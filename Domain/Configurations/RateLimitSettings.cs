namespace Domain.Configurations;

internal sealed class RateLimitSettings
{
    public bool EnableRateLimit { get; set; } = true;
    public Dictionary<string, RateLimitPolicy> Policies { get; set; } = new();
}

internal sealed class RateLimitPolicy
{
    public int PermitLimit { get; set; }
    public int WindowSizeSeconds { get; set; }
}