using Domain.Exceptions;
using Domain.Interfaces.Pipelines;
using Microsoft.Extensions.Logging;

namespace AI.Pipelines.Builder;

/// <summary>
/// Builder imutável de pipelines tipados. Cada <c>.Then</c> retorna um NOVO builder com
/// tipos atualizados, preservando o tipo da raiz <typeparamref name="TRoot"/> e o tipo
/// atual <typeparamref name="TCurrent"/>.
///
/// Não use diretamente — comece via <see cref="Pipeline.Start{TIn}"/>.
/// </summary>
public sealed class PipelineBuilder<TRoot, TCurrent>
{
    private readonly Func<TRoot, CancellationToken, Task<TCurrent>> _chain;
    internal PipelineBuilder(Func<TRoot, CancellationToken, Task<TCurrent>> chain)
        => _chain = chain;

    public PipelineBuilder<TRoot, TNext> Then<TNext>(
        Func<TCurrent, CancellationToken, Task<TNext>> next)
        => new(async (input, ct) =>
        {
            var current = await _chain(input, ct);
            return await next(current, ct);
        });

    public PipelineBuilder<TRoot, TNext> Then<TNext>(Func<TCurrent, TNext> next)
        => Then((c, _) => Task.FromResult(next(c)));

    public PipelineBuilder<TRoot, TCurrent> Tap(Action<TCurrent> action)
        => Then(c => { action(c); return c; });

    public PipelineBuilder<TRoot, TCurrent> Tap(Func<TCurrent, CancellationToken, Task> action)
        => Then(async (c, ct) => { await action(c, ct); return c; });

    public PipelineBuilder<TRoot, TCurrent> Log(ILogger logger, string template)
        => Tap(c => logger.LogInformation(template, c));

    public PipelineBuilder<TRoot, TCurrent> Validate(
        Func<TCurrent, bool> predicate,
        string errorMessage)
        => Then(c =>
        {
            if (!predicate(c))
                throw new PipelineValidationException(errorMessage);
            return c;
        });

    public PipelineBuilder<TRoot, TNext> Branch<TNext>(
        Func<TCurrent, bool> predicate,
        Func<TCurrent, CancellationToken, Task<TNext>> ifTrue,
        Func<TCurrent, CancellationToken, Task<TNext>> ifFalse)
        => Then((c, ct) => predicate(c) ? ifTrue(c, ct) : ifFalse(c, ct));

    public PipelineBuilder<TRoot, TNext> TryOr<TNext>(
        Func<TCurrent, CancellationToken, Task<TNext>> tryStep,
        TNext fallback)
        => Then(async (c, ct) =>
        {
            try
            { return await tryStep(c, ct); }
            catch { return fallback; }
        });

    public IPipeline<TRoot, TCurrent> Build()
        => new ChainedPipeline<TRoot, TCurrent>(_chain);
}
