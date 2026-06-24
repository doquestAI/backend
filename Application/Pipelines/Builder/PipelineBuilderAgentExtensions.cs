using Domain.Interfaces.Agents;

namespace Application.Pipelines.Builder;

/// <summary>
/// Extensões que integram <see cref="IAgent{TIn,TOut}"/> e <see cref="IStreamingAgent{TIn}"/>
/// como passos de pipeline. Usar:
/// <code>
/// .ThenAgent(myAgent, sessionKey: "user-123")
/// </code>
/// </summary>
public static class PipelineBuilderAgentExtensions
{
    /// <summary>Encadeia uma chamada a um <see cref="IAgent{TCurrent,TOut}"/>.</summary>
    public static PipelineBuilder<TRoot, TOut> ThenAgent<TRoot, TCurrent, TOut>(
        this PipelineBuilder<TRoot, TCurrent> builder,
        IAgent<TCurrent, TOut> agent,
        string? sessionKey = null)
        => builder.Then((input, ct) => agent.RunAsync(input, sessionKey, ct));

    /// <summary>
    /// Encadeia uma chamada streaming a um <see cref="IStreamingAgent{TCurrent}"/>,
    /// agregando o resultado em uma única string. Para consumir os chunks individualmente,
    /// use o agente direto na implementação do pipeline streaming.
    /// </summary>
    public static PipelineBuilder<TRoot, string> ThenStreamingAgent<TRoot, TCurrent>(
        this PipelineBuilder<TRoot, TCurrent> builder,
        IStreamingAgent<TCurrent> agent,
        string? sessionKey = null)
        => builder.Then(async (input, ct) =>
        {
            var sb = new System.Text.StringBuilder();
            await foreach (var chunk in agent.RunStreamingAsync(input, sessionKey, ct))
                sb.Append(chunk);
            return sb.ToString();
        });
}
