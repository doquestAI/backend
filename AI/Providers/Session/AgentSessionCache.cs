using Microsoft.Agents.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AI.Providers.Session;

/// <summary>
/// Persiste e hidrata <see cref="AgentSession"/> via <see cref="IDistributedCache"/>
/// usando os métodos nativos do MAF (<see cref="AIAgent.SerializeSessionAsync"/> /
/// <see cref="AIAgent.DeserializeSessionAsync"/>).
///
/// Substitui o antigo DistributedCacheAgentSession custom — agora seguimos o
/// contrato oficial do Microsoft Agent Framework.
/// </summary>
public sealed class AgentSessionCache(
    IDistributedCache cache,
    ILogger<AgentSessionCache> logger)
{
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        SlidingExpiration = TimeSpan.FromHours(2),
    };

    /// <summary>
    /// Carrega a sessão do cache (se existir) ou cria uma nova compatível com o agente.
    /// </summary>
    public async Task<AgentSession> LoadOrCreateAsync(
        AIAgent agent,
        string sessionKey,
        CancellationToken ct = default)
    {
        var json = await cache.GetStringAsync(Key(sessionKey), ct);

        if (json is null)
        {
            logger.LogDebug("Sessão {SessionKey} não encontrada — criando nova.", sessionKey);
            return await agent.CreateSessionAsync(ct);
        }

        try
        {
            using var doc = JsonDocument.Parse(json);
            return await agent.DeserializeSessionAsync(doc.RootElement, cancellationToken: ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Falha ao desserializar sessão {SessionKey} — criando nova.", sessionKey);
            return await agent.CreateSessionAsync(ct);
        }
    }

    /// <summary>
    /// Serializa a sessão atual e persiste no cache distribuído.
    /// </summary>
    public async Task SaveAsync(
        AIAgent agent,
        AgentSession session,
        string sessionKey,
        CancellationToken ct = default)
    {
        var element = await agent.SerializeSessionAsync(session, cancellationToken: ct);
        var json = element.GetRawText();
        await cache.SetStringAsync(Key(sessionKey), json, CacheOptions, ct);
        logger.LogDebug("Sessão {SessionKey} persistida ({Bytes} bytes).", sessionKey, json.Length);
    }

    public Task RemoveAsync(string sessionKey, CancellationToken ct = default)
        => cache.RemoveAsync(Key(sessionKey), ct);

    private static string Key(string sessionKey) => $"agent-session:{sessionKey}";
}
