namespace Domain.Configurations;

internal sealed class CacheSettings
{
    public CacheProvider Provider { get; set; } = CacheProvider.Memory;
    public int DefaultExpirationMinutes { get; set; } = 30;
    public RedisSettings? Redis { get; set; }
}

internal sealed class RedisSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string InstanceName { get; set; } = "Doquest_";
}

public enum CacheProvider
{
    Memory = 0,
    Redis = 1
}