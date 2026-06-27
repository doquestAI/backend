using AI.Pipelines.Builder;
using Domain.Agents.Enem;
using Domain.Interfaces.Agents;
using Domain.Interfaces.Context;
using Domain.Interfaces.Pipelines.Enem;
using Microsoft.Extensions.Logging;

namespace AI.Pipelines.Enem;

/// <summary>Pipeline: Validar → Logar → FeedbackAgent → Logar resultado.</summary>
internal sealed class GradeAnswerPipeline(
    IAgent<FeedbackRequest, FeedbackResult> agent,
    IUserContext userContext,
    ILogger<GradeAnswerPipeline> logger) : IGradeAnswerPipeline
{
    public Task<FeedbackResult> RunAsync(FeedbackRequest input, CancellationToken cancellationToken = default)
    {
        var sessionKey = $"enem:feedback:{userContext.UserId}";

        return Pipeline.Start<FeedbackRequest>()
            .Validate(r => !string.IsNullOrWhiteSpace(r.Question), "Questão é obrigatória.")
            .Validate(r => !string.IsNullOrWhiteSpace(r.StudentAnswer), "Resposta do aluno é obrigatória.")
            .Validate(r => !string.IsNullOrWhiteSpace(r.CorrectAnswer), "Gabarito é obrigatório.")
            .Tap(r => logger.LogInformation(
                "[GradeAnswer] User={UserId} Area={Area}", userContext.UserId, r.Area))
            .ThenAgent(agent, sessionKey)
            .Tap(f => logger.LogInformation(
                "[GradeAnswer] IsCorrect={IsCorrect} Score={Score}", f.IsCorrect, f.Score))
            .Build()
            .RunAsync(input, cancellationToken);
    }
}
