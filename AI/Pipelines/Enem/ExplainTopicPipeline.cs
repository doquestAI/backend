using AI.Pipelines.Builder;
using Domain.Agents.Enem;
using Domain.Interfaces.Agents;
using Domain.Interfaces.Context;
using Domain.Interfaces.Pipelines.Enem;
using Microsoft.Extensions.Logging;

namespace AI.Pipelines.Enem;

/// <summary>Pipeline: Validar → Logar → ExplainerAgent → Garantir resposta não vazia.</summary>
internal sealed class ExplainTopicPipeline(
    IAgent<ExplainRequest, string> agent,
    IUserContext userContext,
    ILogger<ExplainTopicPipeline> logger) : IExplainTopicPipeline
{
    public Task<string> RunAsync(ExplainRequest input, CancellationToken cancellationToken = default)
    {
        var sessionKey = $"enem:explain:{userContext.UserId}";

        return Pipeline.Start<ExplainRequest>()
            .Validate(r => !string.IsNullOrWhiteSpace(r.Topic), "Tópico é obrigatório.")
            .Validate(r => r.Topic.Length <= 500, "Tópico não pode exceder 500 caracteres.")
            .Tap(r => logger.LogInformation(
                "[ExplainTopic] User={UserId} Topic={Topic}", userContext.UserId, r.Topic))
            .ThenAgent(agent, sessionKey)
            .Validate(s => !string.IsNullOrWhiteSpace(s), "Agente retornou resposta vazia.")
            .Tap(s => logger.LogInformation(
                "[ExplainTopic] Response length={Length} chars", s.Length))
            .Build()
            .RunAsync(input, cancellationToken);
    }
}
