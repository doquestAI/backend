using AI.Providers.Session;
using Domain.Interfaces.Agents;
using Microsoft.Agents.AI;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Agents.Enem;

/// <summary>Responde dúvidas gerais sobre o ENEM.</summary>
internal sealed class HelperEnemAgent(
    [FromKeyedServices(AgentKeys.Helper)] AIAgent inner,
    AgentSessionCache sessionCache)
    : EnemTextAgent<string>(inner, sessionCache),
      IAgent<string, string>,
      IStreamingAgent<string>
{
    protected override string FormatInput(string input) => input;
}
