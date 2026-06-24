using Domain.ValueObjects;

namespace Domain.Interfaces.Agents;

public interface IAgentSession
{
    string SessionId { get; }
    string UserId { get; }
    string AgentKey { get; }

    Task AddMessageAsync(AgentMessage message, CancellationToken ct = default);
    Task<IReadOnlyList<AgentMessage>> GetHistoryAsync(int maxMessages = 20, CancellationToken ct = default);
    Task ClearAsync(CancellationToken ct = default);
}