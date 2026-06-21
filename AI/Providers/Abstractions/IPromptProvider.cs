namespace AI.Providers.Abstractions;

internal interface IPromptProvider
{
    Task<string> GetPromptAsync(string promptName, CancellationToken cancellationToken = default);
}
