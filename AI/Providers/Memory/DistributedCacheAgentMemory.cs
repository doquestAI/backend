using Domain.Interfaces.Agents;
using Microsoft.Extensions.Caching.Distributed;

namespace AI.Providers.Memory;

/// <summary>
/// Memória semântica simples via IDistributedCache.
/// Para busca semântica real, substitua SearchAsync por IVectorStore + embeddings.
/// </summary>
internal sealed class DistributedCacheAgentMemory(IDistributedCache cache) : IAgentMemory
{
    private static string Key(string k) => $"memory:{k}";

    public Task SaveFactAsync(string key, string content, CancellationToken ct = default)
        => cache.SetStringAsync(Key(key), content,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
            }, ct);

    public async Task<string?> RecallAsync(string key, CancellationToken ct = default)
        => await cache.GetStringAsync(Key(key), ct);

    public Task<IReadOnlyList<string>> SearchAsync(
        string query, int topK = 5, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());

    public Task ForgetAsync(string key, CancellationToken ct = default)
        => cache.RemoveAsync(Key(key), ct);
}
