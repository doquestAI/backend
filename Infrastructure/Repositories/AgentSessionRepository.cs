using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class AgentSessionRepository(CoreDbContext db) : IAgentSessionRepository
{
    public Task<string?> FindJsonAsync(string sessionKey, CancellationToken ct = default)
        => db.AgentSessions
            .Where(s => s.SessionKey == sessionKey)
            .Select(s => s.SessionJson)
            .FirstOrDefaultAsync(ct);

    public async Task UpsertAsync(
        string sessionKey, string agentName, string sessionJson, CancellationToken ct = default)
    {
        var existing = await db.AgentSessions
            .FirstOrDefaultAsync(s => s.SessionKey == sessionKey, ct);

        if (existing is null)
        {
            db.AgentSessions.Add(new AgentSessionRecord
            {
                Id = Guid.NewGuid(),
                SessionKey = sessionKey,
                AgentName = agentName,
                SessionJson = sessionJson,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });
        }
        else
        {
            existing.SessionJson = sessionJson;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(ct);
    }

    public Task RemoveAsync(string sessionKey, CancellationToken ct = default)
        => db.AgentSessions
            .Where(s => s.SessionKey == sessionKey)
            .ExecuteDeleteAsync(ct);
}
