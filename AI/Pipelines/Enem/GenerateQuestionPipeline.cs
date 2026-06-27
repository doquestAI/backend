using AI.Pipelines.Builder;
using Domain.Agents.Enem;
using Domain.Interfaces.Agents;
using Domain.Interfaces.Context;
using Domain.Interfaces.Pipelines.Enem;
using Microsoft.Extensions.Logging;

namespace AI.Pipelines.Enem;

/// <summary>
/// Pipeline: Validar → Logar → QuestionAgent (sessão por usuário) → Verificar saída.
/// </summary>
internal sealed class GenerateQuestionPipeline(
    IAgent<QuestionRequest, EnemQuestion> agent,
    IUserContext userContext,
    ILogger<GenerateQuestionPipeline> logger) : IGenerateQuestionPipeline
{
    public Task<EnemQuestion> RunAsync(QuestionRequest input, CancellationToken cancellationToken = default)
    {
        var sessionKey = $"enem:question:{userContext.UserId}";

        return Pipeline.Start<QuestionRequest>()
            .Validate(r => !string.IsNullOrWhiteSpace(r.Topic), "Tópico é obrigatório.")
            .Validate(r => !string.IsNullOrWhiteSpace(r.Area), "Área é obrigatória.")
            .Tap(r => logger.LogInformation(
                "[GenerateQuestion] User={UserId} Topic={Topic} Area={Area}",
                userContext.UserId, r.Topic, r.Area))
            .ThenAgent(agent, sessionKey)
            .Validate(q => !string.IsNullOrWhiteSpace(q.Statement), "Agente retornou questão sem enunciado.")
            .Validate(q => q.Options.Count >= 2, "Questão deve ter pelo menos 2 alternativas.")
            .Tap(q => logger.LogInformation(
                "[GenerateQuestion] Generated {Options} options for {Area}",
                q.Options.Count, q.Area))
            .Build()
            .RunAsync(input, cancellationToken);
    }
}
