using AI.Agents.Enem;
using Domain.Interfaces.Agents;
using Domain.Interfaces.Context;
using Domain.Interfaces.Pipelines.Enem;
using Domain.Pipelines;
using Domain.Pipelines.Steps;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace AI.Pipelines.Enem;

/// <summary>
/// Pipeline: Validar → Logar → HelperAgent → Validar saída.
/// Suporta também streaming (SSE) — bypassa a iteração de steps pois precisa preservar
/// o <see cref="IAsyncEnumerable{T}"/> ao longo do caminho.
/// </summary>
internal sealed class AskHelperPipeline : Pipeline<string, string>, IAskHelperPipeline
{
    private readonly HelperEnemAgent _agent;
    private readonly IUserContext _userContext;
    private readonly ILogger<AskHelperPipeline> _logger;

    public AskHelperPipeline(
        HelperEnemAgent agent,
        IUserContext userContext,
        ILogger<AskHelperPipeline> logger) : base("AskHelper")
    {
        _agent = agent;
        _userContext = userContext;
        _logger = logger;

        var sessionKey = $"enem:helper:{userContext.UserId}";

        AddStep(new ValidationStep<string>(
                "Question", q => !string.IsNullOrWhiteSpace(q),
                "Pergunta é obrigatória."))
            .AddStep(new ValidationStep<string>(
                "Question", q => q.Length <= 2000,
                "Pergunta não pode exceder 2000 caracteres."))
            .AddStep(new LoggingStep(
                "LogInput", logger,
                ctx => $"[AskHelper] User={userContext.UserId} QLen={((string)ctx.CurrentValue!).Length}"))
            .AddStep(new TextAgentStep(agent, sessionKey))
            .AddStep(new ValidationStep<string>(
                "Agent", a => !string.IsNullOrWhiteSpace(a),
                "Agente retornou resposta vazia."));
    }

    public async IAsyncEnumerable<string> RunStreamingAsync(
        string input,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            yield return "[validation] Pergunta é obrigatória.";
            yield break;
        }
        if (input.Length > 2000)
        {
            yield return "[validation] Pergunta não pode exceder 2000 caracteres.";
            yield break;
        }

        _logger.LogInformation(
            "[AskHelper:Stream] User={UserId} QuestionLen={Length}",
            _userContext.UserId, input.Length);

        var invocationInput = new AgentInvocationInput(
            Prompt: input,
            SessionKey: $"enem:helper:{_userContext.UserId}");

        await foreach (var chunk in _agent.InvokeStreamingAsync(invocationInput, cancellationToken))
            yield return chunk;

        _logger.LogInformation("[AskHelper:Stream] Stream completed.");
    }
}
