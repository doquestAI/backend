using Domain.Agents.Enem;
using Domain.Interfaces.Agents;
using Domain.Pipelines.Steps;

namespace AI.Pipelines.Enem.Steps;

/// <summary>
/// Step que recebe um <see cref="FeedbackRequest"/> e devolve um <see cref="FeedbackResult"/>
/// com correção, explicação e nota normalizada (0..1).
/// </summary>
internal sealed class GradeAnswerStep(IAgent agent, string? sessionKey = null)
    : AgentStep(agent, sessionKey, name: "GradeAnswer")
{
    protected override string FormatPrompt(object? currentValue)
    {
        if (currentValue is not FeedbackRequest req)
            throw new InvalidOperationException($"Expected FeedbackRequest, got {currentValue?.GetType().Name}");

        return $"Questão: {req.Question}\nResposta do aluno: {req.StudentAnswer}\n" +
               $"Gabarito: {req.CorrectAnswer}\nÁrea: {req.Area}";
    }

    protected override object? ParseResponse(string responseText)
    {
        var correct = responseText.Contains("correta", StringComparison.OrdinalIgnoreCase);
        return new FeedbackResult(
            IsCorrect: correct,
            Explanation: responseText,
            Score: correct ? 1f : 0f);
    }
}
