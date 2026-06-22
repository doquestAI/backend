using AI.Providers.Abstractions;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace AI.Providers;

internal sealed class FilePromptProvider(ILogger<FilePromptProvider> logger) : IPromptProvider
{
    public async Task<PromptTemplate> GetAsync(string key, CancellationToken ct = default)
    {
        var assembly = typeof(FilePromptProvider).Assembly;
        var resourceName = $"AI.Prompts.{key}.yml";

        logger.LogDebug("Carregando prompt do recurso: {Resource}", resourceName);

        await using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"Prompt '{key}' não encontrado como recurso embutido.", resourceName);

        using var reader = new StreamReader(stream);
        var raw = await reader.ReadToEndAsync(ct);
        return new PromptTemplate(raw);
    }

    public Task<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        var assembly = typeof(FilePromptProvider).Assembly;
        var resourceName = $"AI.Prompts.{key}.yml";
        var exists = assembly.GetManifestResourceInfo(resourceName) is not null;
        return Task.FromResult(exists);
    }
}
