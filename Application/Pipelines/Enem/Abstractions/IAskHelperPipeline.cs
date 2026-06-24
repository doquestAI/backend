using Application.Pipelines.Abstractions;

namespace Application.Pipelines.Enem.Abstractions;

/// <summary>Pipeline do agente de dúvidas ENEM. Suporta modo síncrono e streaming SSE.</summary>
public interface IAskHelperPipeline : IPipeline<string, string>, IStreamingPipeline<string>;
