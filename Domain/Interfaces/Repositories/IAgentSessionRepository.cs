namespace Domain.Interfaces.Repositories;

internal interface IAgentSessionRepository
{
    Task<string?> FindJsonAsync(string sessionKey, CancellationToken ct = default);
    Task UpsertAsync(string sessionKey, string agentName, string sessionJson, CancellationToken ct = default);
    Task RemoveAsync(string sessionKey, CancellationToken ct = default);
}
