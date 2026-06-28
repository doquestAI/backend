namespace Infrastructure.Data;

internal sealed class AgentSessionRecord
{
    public Guid Id { get; set; }
    public string SessionKey { get; set; } = default!;
    public string AgentName { get; set; } = default!;
    public string SessionJson { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
