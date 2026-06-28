using Domain.Interfaces.Agents;
using System.Diagnostics;

namespace Domain.Pipelines.Steps;

/// <summary>
/// Step que invoca um <see cref="IAgent"/>. Subclasses sobrescrevem
/// <see cref="FormatPrompt"/> para construir o prompt a partir do contexto e
/// <see cref="ParseResponse"/> para converter a resposta textual no tipo de saída.
///
/// Captura tokens consumidos via <see cref="AgentInvocationResult.Tokens"/> e
/// atribui à <see cref="PipelineMetrics"/> por nome de agent.
/// </summary>
public abstract class AgentStep : PipelineStep
{
    protected IAgent Agent { get; }
    protected string? SessionKey { get; }
    public string AgentName => Agent.AgentName.Value;

    protected AgentStep(IAgent agent, string? sessionKey = null, string? name = null)
        : base(name ?? $"Agent:{agent.AgentName.Value}")
    {
        Agent = agent;
        SessionKey = sessionKey;
    }

    /// <summary>Constrói o prompt enviado ao LLM a partir do valor corrente da pipeline.</summary>
    protected abstract string FormatPrompt(object? currentValue);

    /// <summary>Converte o texto retornado pelo LLM no próximo valor da pipeline.</summary>
    protected abstract object? ParseResponse(string responseText);

    protected sealed override async Task<StepResult> OnExecuteAsync(
        PipelineContext context, CancellationToken cancellationToken)
    {
        var startedAt = DateTime.UtcNow;
        var sw = Stopwatch.StartNew();

        var prompt = FormatPrompt(context.CurrentValue);
        var input = new AgentInvocationInput(
            prompt,
            SessionKey,
            context.RuntimePlugins,
            context.RuntimeMcps);

        var invocation = await Agent.InvokeAsync(input, cancellationToken);
        sw.Stop();

        var parsed = ParseResponse(invocation.Text);
        var metrics = new StepMetrics(
            invocation.Tokens, sw.Elapsed, llmCallCount: 1, startedAt, DateTime.UtcNow);

        return StepResult.Success(parsed, metrics);
    }
}

/// <summary>
/// AgentStep simples: pega o valor corrente (string) e devolve o texto cru do LLM.
/// Use para o caso trivial input texto → output texto sem parsing.
/// </summary>
public sealed class TextAgentStep : AgentStep
{
    public TextAgentStep(IAgent agent, string? sessionKey = null)
        : base(agent, sessionKey) { }

    protected override string FormatPrompt(object? currentValue) =>
        currentValue?.ToString() ?? string.Empty;

    protected override object? ParseResponse(string responseText) => responseText;
}
