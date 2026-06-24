using Application.Pipelines.Abstractions;
using Microsoft.Extensions.Logging;

namespace Application.Pipelines.Builder;

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

    // ── Composição principal ────────────────────────────────────────────────

    /// <summary>Encadeia um passo assíncrono que transforma <typeparamref name="TCurrent"/> em <typeparamref name="TNext"/>.</summary>
    public PipelineBuilder<TRoot, TNext> Then<TNext>(
        Func<TCurrent, CancellationToken, Task<TNext>> next)
        => new(async (input, ct) =>
        {
            var current = await _chain(input, ct);
            return await next(current, ct);
        });

    /// <summary>Encadeia um passo síncrono.</summary>
    public PipelineBuilder<TRoot, TNext> Then<TNext>(Func<TCurrent, TNext> next)
        => Then((c, _) => Task.FromResult(next(c)));

    // ── Side effects (Tap / Log) ────────────────────────────────────────────

    /// <summary>Executa side-effect sem alterar o valor (logging, métricas).</summary>
    public PipelineBuilder<TRoot, TCurrent> Tap(Action<TCurrent> action)
        => Then(c => { action(c); return c; });

    public PipelineBuilder<TRoot, TCurrent> Tap(Func<TCurrent, CancellationToken, Task> action)
        => Then(async (c, ct) => { await action(c, ct); return c; });

    /// <summary>Loga via <see cref="ILogger"/> com template formatável.</summary>
    public PipelineBuilder<TRoot, TCurrent> Log(ILogger logger, string template)
        => Tap(c => logger.LogInformation(template, c));

    // ── Validação ───────────────────────────────────────────────────────────

    /// <summary>Valida o valor corrente. Falha lança <see cref="PipelineValidationException"/>.</summary>
    public PipelineBuilder<TRoot, TCurrent> Validate(
        Func<TCurrent, bool> predicate,
        string errorMessage)
        => Then(c =>
        {
            if (!predicate(c))
                throw new PipelineValidationException(errorMessage);
            return c;
        });

    // ── Controle de fluxo ───────────────────────────────────────────────────

    /// <summary>Branch condicional: executa <paramref name="ifTrue"/> se o predicado for verdadeiro, senão <paramref name="ifFalse"/>.</summary>
    public PipelineBuilder<TRoot, TNext> Branch<TNext>(
        Func<TCurrent, bool> predicate,
        Func<TCurrent, CancellationToken, Task<TNext>> ifTrue,
        Func<TCurrent, CancellationToken, Task<TNext>> ifFalse)
        => Then((c, ct) => predicate(c) ? ifTrue(c, ct) : ifFalse(c, ct));

    /// <summary>Tenta um passo; se lançar exceção, retorna <paramref name="fallback"/>.</summary>
    public PipelineBuilder<TRoot, TNext> TryOr<TNext>(
        Func<TCurrent, CancellationToken, Task<TNext>> tryStep,
        TNext fallback)
        => Then(async (c, ct) =>
        {
            try { return await tryStep(c, ct); }
            catch { return fallback; }
        });

    // ── Build ───────────────────────────────────────────────────────────────

    public IPipeline<TRoot, TCurrent> Build()
        => new ChainedPipeline<TRoot, TCurrent>(_chain);
}
