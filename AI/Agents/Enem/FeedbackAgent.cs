using AI.Providers.Abstractions;
using Domain.Enums;
using Domain.Interfaces.Agents;
using Domain.ValueObjects;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AI.Agents.Enem;

/// <summary>Corrige e avalia a resposta do candidato.</summary>
internal sealed class FeedbackAgent(
    IChatClient chatClient,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGen,
    IVectorStore vectorStore,
    [FromKeyedServices(PromptProvider.File)] IPromptProvider promptProvider,
    IAgentSession session,
    IAgentMemory memory,
    IOptions<AgentOptions> options,
    ILogger<FeedbackAgent> logger)
    : EnemAgentBase<FeedbackRequest, FeedbackResult>(chatClient, embeddingGen, vectorStore,
                                                      promptProvider, session, memory, options, logger)
{
    protected override string AgentKey => "enem-feedback";

    protected override string ExtractTextFromInput(FeedbackRequest input)
        => $"Questão: {input.Question}\nResposta do aluno: {input.StudentAnswer}\n" +
           $"Gabarito: {input.CorrectAnswer}";

    protected override Task<FeedbackResult> ParseResponseAsync(
        ChatCompletion response, CancellationToken ct)
    {
        var text = response.Message.Text ?? string.Empty;

        return Task.FromResult(new FeedbackResult(
            IsCorrect: text.Contains("correta", StringComparison.OrdinalIgnoreCase),
            Explanation: text,
            Score: text.Contains("correta", StringComparison.OrdinalIgnoreCase) ? 1f : 0f
        ));
    }

    protected override async Task SaveToMemoryAsync(
        FeedbackRequest input, string response, CancellationToken ct)
    {
        var correct = response.Contains("correta", StringComparison.OrdinalIgnoreCase);
        var key = $"performance:{input.Area}:{DateTimeOffset.UtcNow:yyyyMMddHHmmss}";
        await Memory.SaveFactAsync(key, $"Área: {input.Area} | Acertou: {correct}", ct);
    }
}