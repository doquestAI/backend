using Domain.Agents.Enem;
using Domain.Interfaces.Agents;
using Domain.Pipelines.Steps;

namespace AI.Pipelines.Enem.Steps;

/// <summary>
/// Step que recebe um <see cref="ExplainRequest"/> e devolve a explicação como string.
/// </summary>
internal sealed class ExplainTopicStep(IAgent agent, string? sessionKey = null)
    : AgentStep(agent, sessionKey, name: "ExplainTopic")
{
    protected override string FormatPrompt(object? currentValue)
    {
        if (currentValue is not ExplainRequest req)
            throw new InvalidOperationException($"Expected ExplainRequest, got {currentValue?.GetType().Name}");

        return $"Explique detalhadamente sobre: {req.Topic}. Área: {req.Area}";
    }

    protected override object? ParseResponse(string responseText) => responseText;
}
