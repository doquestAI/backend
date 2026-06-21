using AI.Providers.Abstractions;
using Domain.Enums;
using Domain.Interfaces.Agents;

namespace AI.Agents;

internal class HelperEnemAgent(
    [FromKeyedServices(PromptProvider.File)] IPromptProvider promptProvider
)
    : IAgent<string, string>
{
    private PromptTemplate Template { get; set; }
    public async Task<string> RunAsync(string data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
