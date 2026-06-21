using AI.Providers.Abstractions;

namespace AI.Providers;

internal class FilePromptProvider
    : IPromptProvider
{
    public async Task<string> GetPromptAsync(string agentName, CancellationToken cancellationToken = default)
    {
        var assembly = typeof(FilePromptProvider).Assembly;

        var resourceName = $"AI.Prompts.{agentName}.md";
        await using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new FileNotFoundException("Prompt not found", agentName);

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken);
    }
}
