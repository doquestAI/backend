namespace Domain.Agents;

internal sealed class AgentMetadata
{
    public string Description { get; }
    public IReadOnlyDictionary<string, string> Tags { get; }
    public DateTime CreatedAt { get; }

    public AgentMetadata(string description, IReadOnlyDictionary<string, string>? tags = null)
    {
        Description = description;
        Tags = tags ?? new Dictionary<string, string>();
        CreatedAt = DateTime.UtcNow;
    }
}
