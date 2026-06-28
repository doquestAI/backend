using Domain.ValueObjects;

namespace Domain.Interfaces.Prompts;

/// <summary>
/// Carrega templates de prompt de diversas fontes (arquivo, banco, remoto).
/// Registrado com Keyed DI pelo enum <c>Domain.Enums.PromptProvider</c>.
/// </summary>
internal interface IPromptProvider
{
    Task<PromptTemplate> GetAsync(string key, CancellationToken ct = default);
    Task<bool> ExistsAsync(string key, CancellationToken ct = default);
}
