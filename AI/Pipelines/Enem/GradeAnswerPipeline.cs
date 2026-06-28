using AI.Agents.Enem;
using AI.Pipelines.Enem.Steps;
using Domain.Agents.Enem;
using Domain.Interfaces.Context;
using Domain.Interfaces.Pipelines.Enem;
using Domain.Pipelines;
using Domain.Pipelines.Steps;
using Microsoft.Extensions.Logging;

namespace AI.Pipelines.Enem;

/// <summary>Pipeline: Validar → Logar → FeedbackAgent → Logar saída.</summary>
internal sealed class GradeAnswerPipeline : Pipeline<FeedbackRequest, FeedbackResult>, IGradeAnswerPipeline
{
    public GradeAnswerPipeline(
        FeedbackAgent agent,
        IUserContext userContext,
        ILogger<GradeAnswerPipeline> logger) : base("GradeAnswer")
    {
        var sessionKey = $"enem:feedback:{userContext.UserId}";

        AddStep(new ValidationStep<FeedbackRequest>(
                nameof(FeedbackRequest.Question),
                r => !string.IsNullOrWhiteSpace(r.Question),
                "Questão é obrigatória."))
            .AddStep(new ValidationStep<FeedbackRequest>(
                nameof(FeedbackRequest.StudentAnswer),
                r => !string.IsNullOrWhiteSpace(r.StudentAnswer),
                "Resposta do aluno é obrigatória."))
            .AddStep(new ValidationStep<FeedbackRequest>(
                nameof(FeedbackRequest.CorrectAnswer),
                r => !string.IsNullOrWhiteSpace(r.CorrectAnswer),
                "Gabarito é obrigatório."))
            .AddStep(new LoggingStep(
                "LogInput", logger,
                ctx => $"[GradeAnswer] User={userContext.UserId}"))
            .AddStep(new GradeAnswerStep(agent, sessionKey));
    }
}
