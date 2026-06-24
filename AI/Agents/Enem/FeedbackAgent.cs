using AI.Agents.Base;
using AI.Providers.Session;
using Domain.Agents.Enem;
using Domain.Interfaces.Agents;
using Microsoft.Agents.AI;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Agents.Enem;

/// <summary>Corrige e avalia a resposta do candidato.</summary>
internal sealed class FeedbackAgent(
    [FromKeyedServices(AgentKeys.Feedback)] AIAgent inner,
    AgentSessionCache sessionCache)
    : AgentWrapperBase<FeedbackRequest, FeedbackResult>(inner, sessionCache),
      IAgent<FeedbackRequest, FeedbackResult>
{
    protected override string FormatInput(FeedbackRequest input)
        => $"Questão: {input.Question}\nResposta do aluno: {input.StudentAnswer}\nGabarito: {input.CorrectAnswer}\nÁrea: {input.Area}";

    protected override FeedbackResult ParseResponse(AgentResponse response)
    {
        var text = ResponseText(response);
        var correct = text.Contains("correta", StringComparison.OrdinalIgnoreCase);

        return new FeedbackResult(
            IsCorrect: correct,
            Explanation: text,
            Score: correct ? 1f : 0f);
    }
}
