using AI.Agents.Base;
using AI.Providers.Session;
using Domain.Agents.Enem;
using Domain.Interfaces.Agents;
using Microsoft.Agents.AI;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

namespace AI.Agents.Enem;

/// <summary>Gera questões no estilo ENEM.</summary>
internal sealed partial class QuestionAgent(
    [FromKeyedServices(AgentKeys.QuestionGenerator)] AIAgent inner,
    AgentSessionCache sessionCache)
    : AgentWrapperBase<QuestionRequest, EnemQuestion>(inner, sessionCache),
      IAgent<QuestionRequest, EnemQuestion>
{
    protected override string FormatInput(QuestionRequest input)
        => $"Crie uma questão ENEM sobre: {input.Topic}. Dificuldade: {input.Difficulty}. Área: {input.Area}";

    protected override EnemQuestion ParseResponse(AgentResponse response)
    {
        var text = ResponseText(response);

        return new EnemQuestion(
            Statement: ExtractSection(text, "enunciado"),
            Options: ExtractOptions(text),
            CorrectKey: ExtractSection(text, "resposta"),
            Explanation: ExtractSection(text, "explicacao"),
            Area: ExtractSection(text, "area"));
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
            return Array.Empty<string>();

        return match.Groups[1].Value
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrEmpty(l))
            .ToArray();
    }

    [GeneratedRegex(@"<opcoes>(.*?)</opcoes>", RegexOptions.Singleline)]
    private static partial Regex OptionsRegex();
}
