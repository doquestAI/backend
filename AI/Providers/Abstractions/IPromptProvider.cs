using Domain.ValueObjects;

namespace AI.Providers.Abstractions;

/// <summary>
/// Carrega templates de prompt de diversas fontes (arquivo, banco, remoto).
/// Registrado com Keyed DI pelo enum PromptProvider.
/// </summary>
public interface IPromptProvider
{
    Task<PromptTemplate> GetAsync(string key, CancellationToken ct = default);
    Task<bool> ExistsAsync(string key, CancellationToken ct = default);
}
