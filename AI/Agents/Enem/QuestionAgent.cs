using System.Text.RegularExpressions;
using AI.Providers.Abstractions;
using Domain.Enums;
using Domain.Interfaces.Agents;
using Domain.ValueObjects;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AI.Agents.Enem;

/// <summary>Gera questões no estilo ENEM.</summary>
internal sealed class QuestionAgent(
    IChatClient                                    chatClient,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGen,
    IVectorStore                                   vectorStore,
    [FromKeyedServices(PromptProvider.File)] IPromptProvider promptProvider,
    IAgentSession                                  session,
    IAgentMemory                                   memory,
    IOptions<AgentOptions>                         options,
    ILogger<QuestionAgent>                         logger)
    : EnemAgentBase<QuestionRequest, EnemQuestion>(chatClient, embeddingGen, vectorStore,
                                                    promptProvider, session, memory, options, logger)
{
    protected override string AgentKey => "enem-question-generator";

    protected override string ExtractTextFromInput(QuestionRequest input)
        => $"Crie uma questão ENEM sobre: {input.Topic}. Dificuldade: {input.Difficulty}. Área: {input.Area}";

    protected override Task<EnemQuestion> ParseResponseAsync(
        ChatCompletion response, CancellationToken ct)
    {
        var text = response.Message.Text ?? string.Empty;

        var question = new EnemQuestion(
            Statement:   ExtractSection(text, "enunciado"),
            Options:     ExtractOptions(text),
            CorrectKey:  ExtractSection(text, "resposta"),
            Explanation: ExtractSection(text, "explicacao"),
            Area:        ExtractSection(text, "area")
        );

        return Task.FromResult(question);
    }

    private static string ExtractSection(string text, string tag) =>
        Regex.Match(text, $@"<{tag}>(.*?)</{tag}>", RegexOptions.Singleline)
             .Groups[1].Value.Trim();

    private static IReadOnlyList<string> ExtractOptions(string text)
    {
        var match = Regex.Match(text, @"<opcoes>(.*?)</opcoes>", RegexOptions.Singleline);
        if (!match.Success) return Array.Empty<string>();
        return match.Groups[1].Value
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrEmpty(l))
            .ToArray();
    }
}
