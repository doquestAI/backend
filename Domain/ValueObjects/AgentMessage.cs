using Domain.Enums;

namespace Domain.ValueObjects;

public sealed record AgentMessage(
    AgentRole Role,
    string Content,
    DateTimeOffset CreatedAt = default
)
{
    public static AgentMessage System(string content) => new(AgentRole.System, content, DateTimeOffset.UtcNow);
    public static AgentMessage User(string content) => new(AgentRole.User, content, DateTimeOffset.UtcNow);
    public static AgentMessage Assistant(string content) => new(AgentRole.Assistant, content, DateTimeOffset.UtcNow);
}