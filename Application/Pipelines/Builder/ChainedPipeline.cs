using Application.Pipelines.Abstractions;

namespace Application.Pipelines.Builder;

/// <summary>Implementação concreta de <see cref="IPipeline{TIn,TOut}"/> usando um delegado encadeado.</summary>
internal sealed class ChainedPipeline<TIn, TOut>(
    Func<TIn, CancellationToken, Task<TOut>> chain) : IPipeline<TIn, TOut>
{
    public Task<TOut> RunAsync(TIn input, CancellationToken cancellationToken = default)
        => chain(input, cancellationToken);
}
