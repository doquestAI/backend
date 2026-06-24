using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AI.Providers.Context;

/// <summary>
/// <see cref="AIContextProvider"/> que mantém memória de longo prazo do agente em
/// <see cref="IDistributedCache"/> (Redis, SQL, in-memory).
///
/// - <c>ProvideAIContextAsync</c>: carrega fatos da memória e injeta como <see cref="AIContext.Instructions"/>.
/// - <c>StoreAIContextAsync</c>: persiste as últimas mensagens do turno como fatos com TTL.
///
/// Substitui <c>DistributedCacheAgentMemory</c> + bloco hardcoded de memória em <c>AgentBase</c>.
/// </summary>
public sealed class DistributedCacheMemoryProvider(
    IDistributedCache cache,
    string memoryNamespace,
    ILogger<DistributedCacheMemoryProvider> logger,
    int recallLimit = 5,
    TimeSpan? ttl = null)
    : AIContextProvider
{
    private readonly TimeSpan _ttl = ttl ?? TimeSpan.FromDays(30);

    private string IndexKey => $"memory:{memoryNamespace}:index";
    private string FactKey(string id) => $"memory:{memoryNamespace}:fact:{id}";

    protected override async ValueTask<AIContext> ProvideAIContextAsync(
        InvokingContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var indexJson = await cache.GetStringAsync(IndexKey, cancellationToken);
            if (string.IsNullOrEmpty(indexJson))
                return new AIContext();

            var ids = JsonSerializer.Deserialize<List<string>>(indexJson) ?? [];
            if (ids.Count == 0)
                return new AIContext();

            var facts = new List<string>(Math.Min(recallLimit, ids.Count));
            foreach (var id in ids.TakeLast(recallLimit))
            {
                var fact = await cache.GetStringAsync(FactKey(id), cancellationToken);
                if (!string.IsNullOrEmpty(fact))
                    facts.Add(fact);
            }

            if (facts.Count == 0)
                return new AIContext();

            var formatted = string.Join("\n", facts.Select((f, i) => $"- [{i + 1}] {f}"));
            return new AIContext
            {
                Instructions = $"### Memória de longo prazo\n{formatted}",
            };
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Falha ao carregar memória do namespace {Namespace}.", memoryNamespace);
            return new AIContext();
        }
    }

    protected override async ValueTask StoreAIContextAsync(
        InvokedContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var lastUserText = context.RequestMessages
                ?.LastOrDefault(m => m.Role == ChatRole.User)?.Text;
            var lastAssistantText = context.ResponseMessages
                ?.LastOrDefault(m => m.Role == ChatRole.Assistant)?.Text;

            if (string.IsNullOrWhiteSpace(lastUserText) || string.IsNullOrWhiteSpace(lastAssistantText))
                return;

            var fact = $"Q: {Truncate(lastUserText, 200)}\nA: {Truncate(lastAssistantText, 400)}";
            var id = Guid.NewGuid().ToString("N");

            await cache.SetStringAsync(
                FactKey(id),
                fact,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _ttl },
                cancellationToken);

            await UpdateIndexAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Falha ao salvar memória no namespace {Namespace}.", memoryNamespace);
        }
    }

    private async Task UpdateIndexAsync(string newId, CancellationToken ct)
    {
        var indexJson = await cache.GetStringAsync(IndexKey, ct);
        var ids = string.IsNullOrEmpty(indexJson)
            ? []
            : JsonSerializer.Deserialize<List<string>>(indexJson) ?? new List<string>();

        ids.Add(newId);
        if (ids.Count > 100)
            ids.RemoveRange(0, ids.Count - 100);

        await cache.SetStringAsync(
            IndexKey,
            JsonSerializer.Serialize(ids),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _ttl },
            ct);
    }

    private static string Truncate(string s, int max)
        => s.Length <= max ? s : s[..max] + "…";
}
