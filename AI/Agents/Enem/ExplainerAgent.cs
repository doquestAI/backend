using AI.Providers.Session;
using Domain.Agents.Enem;
using Domain.Interfaces.Agents;
using Microsoft.Agents.AI;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Agents.Enem;

/// <summary>Explica teoria de um tópico do ENEM em detalhes.</summary>
internal sealed class ExplainerAgent(
    [FromKeyedServices(AgentKeys.Explainer)] AIAgent inner,
    AgentSessionCache sessionCache)
    : EnemTextAgent<ExplainRequest>(inner, sessionCache),
      IAgent<ExplainRequest, string>,
      IStreamingAgent<ExplainRequest>
{
    protected override string FormatInput(ExplainRequest input)
        => $"Explique detalhadamente sobre: {input.Topic}. Área: {input.Area}";
}
