using AI.Providers.Abstractions;
using Domain.Enums;
using Domain.Interfaces.Agents;

namespace AI.Agents;

internal class HelperEnemAgent(
    [FromKeyedServices(PromptProvider.File)] IPromptProvider promptProvider
)
    : IAgent<string, string>
{
    private const string AgentName = "HelperEnemAgent";
    private const string PromptTemplate = "You are a helpful assistant for the ENEM exam. Your task is to help the user understand and solve questions related to the ENEM exam.";


    public async Task<string> RunAsync(string data, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
