namespace Domain.Interfaces.Agents;

public interface IAgentMemory
{
    Task SaveFactAsync(string key, string content, CancellationToken ct = default);
    Task<string?> RecallAsync(string key, CancellationToken ct = default);
    Task<IReadOnlyList<string>> SearchAsync(string query, int topK = 5, CancellationToken ct = default);
    Task ForgetAsync(string key, CancellationToken ct = default);
}