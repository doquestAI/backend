using Domain.Pipelines;

namespace Domain.Interfaces.Pipelines;

/// <summary>
/// Contrato mínimo de uma Pipeline tipada. Implementado por <c>Pipeline&lt;TIn,TOut&gt;</c>
/// (em <c>Domain.Pipelines</c>) e pelas pipelines concretas (ex: <c>GenerateQuestionPipeline</c>).
/// </summary>
public interface IPipeline<in TIn, TOut>
{
    Task<PipelineResult<TOut>> RunAsync(TIn input, CancellationToken cancellationToken = default);
}

/// <summary>Pipeline que produz resposta em streaming (texto incremental, SSE).</summary>
public interface IStreamingPipeline<in TIn>
{
    IAsyncEnumerable<string> RunStreamingAsync(TIn input, CancellationToken cancellationToken = default);
}
