namespace Domain.Configurations;

public sealed class RateLimitSettings
{
    public bool EnableRateLimit { get; set; } = true;
    public Dictionary<string, RateLimitPolicy> Policies { get; set; } = new();
}

public sealed class RateLimitPolicy
{
    public int PermitLimit { get; set; }
    public int WindowSizeSeconds { get; set; }
}
