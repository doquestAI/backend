using Domain.Interfaces.Repositories;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AI.Providers.Session;

internal sealed class AgentSessionStore(
    IAgentSessionRepository repository,
    ILogger<AgentSessionStore> logger)
{
    public async Task<AgentSession> LoadOrCreateAsync(
        AIAgent agent,
        string sessionKey,
        CancellationToken ct = default)
    {
        var json = await repository.FindJsonAsync(sessionKey, ct);

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

    public async Task SaveAsync(
        AIAgent agent,
        AgentSession session,
        string sessionKey,
        string agentName,
        CancellationToken ct = default)
    {
        var element = await agent.SerializeSessionAsync(session, cancellationToken: ct);
        var json = element.GetRawText();
        await repository.UpsertAsync(sessionKey, agentName, json, ct);
        logger.LogDebug("Sessão {SessionKey} persistida ({Bytes} bytes).", sessionKey, json.Length);
    }

    public Task RemoveAsync(string sessionKey, CancellationToken ct = default)
        => repository.RemoveAsync(sessionKey, ct);
}
