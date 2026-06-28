using Domain.Agents.Enem;
using Domain.Interfaces.Agents;
using Domain.Pipelines.Steps;
using System.Text.RegularExpressions;

namespace AI.Pipelines.Enem.Steps;

/// <summary>
/// Step que recebe um <see cref="QuestionRequest"/>, monta o prompt para o agente
/// e parseia o XML da resposta em <see cref="EnemQuestion"/>.
/// </summary>
internal sealed partial class GenerateQuestionStep(IAgent agent, string? sessionKey = null)
    : AgentStep(agent, sessionKey, name: "GenerateQuestion")
{
    protected override string FormatPrompt(object? currentValue)
    {
        if (currentValue is not QuestionRequest req)
            throw new InvalidOperationException($"Expected QuestionRequest, got {currentValue?.GetType().Name}");

        return $"Crie uma questão ENEM sobre: {req.Topic}. Dificuldade: {req.Difficulty}. Área: {req.Area}";
    }

    protected override object? ParseResponse(string responseText)
    {
        return new EnemQuestion(
            Statement: ExtractSection(responseText, "enunciado"),
            Options: ExtractOptions(responseText),
            CorrectKey: ExtractSection(responseText, "resposta"),
            Explanation: ExtractSection(responseText, "explicacao"),
            Area: ExtractSection(responseText, "area"));
    }

    private static string ExtractSection(string text, string tag)
    {
        var pattern = $@"<{tag}>(.*?)</{tag}>";
        return Regex.Match(text, pattern, RegexOptions.Singleline).Groups[1].Value.Trim();
    }

    private static IReadOnlyList<string> ExtractOptions(string text)
    {
        var match = OptionsRegex().Match(text);
        if (!match.Success)
            return [];

        return match.Groups[1].Value
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrEmpty(l))
            .ToArray();
    }

    [GeneratedRegex(@"<opcoes>(.*?)</opcoes>", RegexOptions.Singleline)]
    private static partial Regex OptionsRegex();
}
