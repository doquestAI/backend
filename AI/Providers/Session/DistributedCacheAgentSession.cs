using Domain.Interfaces.Agents;
using Domain.ValueObjects;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AI.Providers.Session;

/// <summary>
/// Sessão baseada em IDistributedCache (Redis, memória, SQL, etc.).
/// SessionId e UserId devem ser injetados via factory a partir de IHttpContextAccessor.
/// </summary>
internal sealed class DistributedCacheAgentSession(
    IDistributedCache cache,
    ILogger<DistributedCacheAgentSession> logger,
    string sessionId,
    string userId,
    string agentKey)
    : IAgentSession
{
    public string SessionId { get; } = sessionId;
    public string UserId { get; } = userId;
    public string AgentKey { get; } = agentKey;

    private string CacheKey => $"session:{AgentKey}:{UserId}:{SessionId}";

    public async Task AddMessageAsync(AgentMessage message, CancellationToken ct = default)
    {
        var history = (await GetHistoryInternalAsync(ct)).ToList();
        history.Add(message);

        var json = JsonSerializer.Serialize(history);
        await cache.SetStringAsync(CacheKey, json,
            new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(2) }, ct);

        logger.LogDebug("Mensagem adicionada à sessão {SessionId}: Role={Role}", SessionId, message.Role);
    }

    public async Task<IReadOnlyList<AgentMessage>> GetHistoryAsync(
        int maxMessages = 20, CancellationToken ct = default)
    {
        var all = await GetHistoryInternalAsync(ct);
        return all.TakeLast(maxMessages).ToList();
    }

    public Task ClearAsync(CancellationToken ct = default)
        => cache.RemoveAsync(CacheKey, ct);

    private async Task<List<AgentMessage>> GetHistoryInternalAsync(CancellationToken ct)
    {
        var json = await cache.GetStringAsync(CacheKey, ct);
        if (json is null)
            return [];
        return JsonSerializer.Deserialize<List<AgentMessage>>(json) ?? [];
    }
}