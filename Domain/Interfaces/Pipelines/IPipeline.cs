namespace Domain.Interfaces.Pipelines;

/// <summary>
/// Pipeline tipado que recebe <typeparamref name="TIn"/> e produz <typeparamref name="TOut"/>.
/// Implementação concreta vive na camada AI (composto via PipelineBuilder em estilo LangChain).
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
