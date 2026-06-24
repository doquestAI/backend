using Application.Pipelines.Abstractions;
using Application.Pipelines.Builder;
using Application.Pipelines.Enem.Abstractions;
using Domain.Interfaces.Agents;
using Domain.Interfaces.Context;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Application.Pipelines.Enem;

/// <summary>
/// Pipeline: Validar → Logar → HelperAgent (síncrono OU streaming).
///
/// O streaming não passa pelo PipelineBuilder pois precisa preservar o IAsyncEnumerable —
/// implementado direto em <see cref="RunStreamingAsync"/>.
/// </summary>
internal sealed class AskHelperPipeline(
    IAgent<string, string> agent,
    IStreamingAgent<string> streamingAgent,
    IUserContext userContext,
    ILogger<AskHelperPipeline> logger) : IAskHelperPipeline
{
    public Task<string> RunAsync(string input, CancellationToken cancellationToken = default)
    {
        var sessionKey = SessionKey;

        return Pipeline.Start<string>()
            .Validate(q => !string.IsNullOrWhiteSpace(q), "Pergunta é obrigatória.")
            .Validate(q => q.Length <= 2000, "Pergunta não pode exceder 2000 caracteres.")
            .Tap(q => logger.LogInformation(
                "[AskHelper] User={UserId} QuestionLen={Length}", userContext.UserId, q.Length))
            .ThenAgent(agent, sessionKey)
            .Validate(a => !string.IsNullOrWhiteSpace(a), "Agente retornou resposta vazia.")
            .Tap(a => logger.LogInformation(
                "[AskHelper] AnswerLen={Length}", a.Length))
            .Build()
            .RunAsync(input, cancellationToken);
    }

    public async IAsyncEnumerable<string> RunStreamingAsync(
        string input,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new PipelineValidationException("Pergunta é obrigatória.");
        if (input.Length > 2000)
            throw new PipelineValidationException("Pergunta não pode exceder 2000 caracteres.");

        logger.LogInformation(
            "[AskHelper:Stream] User={UserId} QuestionLen={Length}",
            userContext.UserId, input.Length);

        await foreach (var chunk in streamingAgent.RunStreamingAsync(
            input, SessionKey, cancellationToken))
        {
            yield return chunk;
        }

        logger.LogInformation("[AskHelper:Stream] Stream completed.");
    }

    private string SessionKey => $"enem:helper:{userContext.UserId}";
}
