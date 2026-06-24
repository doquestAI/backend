namespace Application.Pipelines.Abstractions;

/// <summary>
/// Pipeline tipado que recebe <typeparamref name="TIn"/> e produz <typeparamref name="TOut"/>.
/// Composto via <see cref="Builder.Pipeline"/>.Start em estilo LangChain Chain.
/// </summary>
public interface IPipeline<in TIn, TOut>
{
    Task<TOut> RunAsync(TIn input, CancellationToken cancellationToken = default);
}

/// <summary>Pipeline que produz resposta em streaming (texto incremental).</summary>
public interface IStreamingPipeline<in TIn>
{
    IAsyncEnumerable<string> RunStreamingAsync(TIn input, CancellationToken cancellationToken = default);
}
