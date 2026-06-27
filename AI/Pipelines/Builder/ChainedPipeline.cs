using Domain.Interfaces.Pipelines;

namespace AI.Pipelines.Builder;

internal sealed class ChainedPipeline<TIn, TOut>(
    Func<TIn, CancellationToken, Task<TOut>> chain) : IPipeline<TIn, TOut>
{
    public Task<TOut> RunAsync(TIn input, CancellationToken cancellationToken = default)
        => chain(input, cancellationToken);
}
