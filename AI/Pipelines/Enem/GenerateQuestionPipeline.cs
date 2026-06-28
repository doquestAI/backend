using AI.Agents.Enem;
using AI.Pipelines.Enem.Steps;
using Domain.Agents.Enem;
using Domain.Interfaces.Context;
using Domain.Interfaces.Pipelines.Enem;
using Domain.Pipelines;
using Domain.Pipelines.Steps;
using Microsoft.Extensions.Logging;

namespace AI.Pipelines.Enem;

/// <summary>
/// Pipeline: Validar → Logar → QuestionAgent → Validar saída.
/// 100% POO — composta no construtor adicionando steps em sequência.
/// </summary>
internal sealed class GenerateQuestionPipeline : Pipeline<QuestionRequest, EnemQuestion>, IGenerateQuestionPipeline
{
    public GenerateQuestionPipeline(
        QuestionAgent agent,
        IUserContext userContext,
        ILogger<GenerateQuestionPipeline> logger) : base("GenerateQuestion")
    {
        var sessionKey = $"enem:question:{userContext.UserId}";

        AddStep(new ValidationStep<QuestionRequest>(
                nameof(QuestionRequest.Topic),
                r => !string.IsNullOrWhiteSpace(r.Topic),
                "Tópico é obrigatório."))
            .AddStep(new ValidationStep<QuestionRequest>(
                nameof(QuestionRequest.Area),
                r => !string.IsNullOrWhiteSpace(r.Area),
                "Área é obrigatória."))
            .AddStep(new LoggingStep(
                "LogInput", logger,
                ctx => $"[GenerateQuestion] User={userContext.UserId} Input={ctx.CurrentValue}"))
            .AddStep(new GenerateQuestionStep(agent, sessionKey))
            .AddStep(new ValidationStep<EnemQuestion>(
                nameof(EnemQuestion.Statement),
                q => !string.IsNullOrWhiteSpace(q.Statement),
                "Agente retornou questão sem enunciado."))
            .AddStep(new ValidationStep<EnemQuestion>(
                nameof(EnemQuestion.Options),
                q => q.Options.Count >= 2,
                "Questão deve ter pelo menos 2 alternativas."));
    }
}
