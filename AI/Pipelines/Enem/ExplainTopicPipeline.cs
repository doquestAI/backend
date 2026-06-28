using AI.Agents.Enem;
using AI.Pipelines.Enem.Steps;
using Domain.Agents.Enem;
using Domain.Interfaces.Context;
using Domain.Interfaces.Pipelines.Enem;
using Domain.Pipelines;
using Domain.Pipelines.Steps;
using Microsoft.Extensions.Logging;

namespace AI.Pipelines.Enem;

/// <summary>Pipeline: Validar → Logar → ExplainerAgent → Validar saída.</summary>
public sealed class ExplainTopicPipeline : Pipeline<ExplainRequest, string>, IExplainTopicPipeline
{
    public ExplainTopicPipeline(
        ExplainerAgent agent,
        IUserContext userContext,
        ILogger<ExplainTopicPipeline> logger) : base("ExplainTopic")
    {
        var sessionKey = $"enem:explain:{userContext.UserId}";

        AddStep(new ValidationStep<ExplainRequest>(
                nameof(ExplainRequest.Topic),
                r => !string.IsNullOrWhiteSpace(r.Topic),
                "Tópico é obrigatório."))
            .AddStep(new ValidationStep<ExplainRequest>(
                nameof(ExplainRequest.Topic),
                r => r.Topic.Length <= 500,
                "Tópico não pode exceder 500 caracteres."))
            .AddStep(new LoggingStep(
                "LogInput", logger,
                ctx => $"[ExplainTopic] User={userContext.UserId} Topic={((ExplainRequest)ctx.CurrentValue!).Topic}"))
            .AddStep(new ExplainTopicStep(agent, sessionKey))
            .AddStep(new ValidationStep<string>(
                "Agent", s => !string.IsNullOrWhiteSpace(s),
                "Agente retornou resposta vazia."));
    }
}
