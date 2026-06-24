namespace Application.Pipelines.Builder;

/// <summary>
/// Entry point para criar pipelines tipados em estilo LangChain Chain:
/// <code>
/// var pipeline = Pipeline.Start&lt;Request&gt;()
///     .Validate(r =&gt; r.IsValid, "Invalid")
///     .Tap(r =&gt; logger.LogInformation("..."))
///     .ThenAgent(agent, sessionKey)
///     .Then(result =&gt; Map(result))
///     .Build();
/// </code>
/// </summary>
public static class Pipeline
{
    /// <summary>Inicia um pipeline cuja entrada é <typeparamref name="TIn"/>.</summary>
    public static PipelineBuilder<TIn, TIn> Start<TIn>()
        => new((input, _) => Task.FromResult(input));
}
