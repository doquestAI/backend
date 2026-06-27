namespace Domain.Interfaces.Pipelines.Enem;

/// <summary>Pipeline do agente de dúvidas ENEM. Suporta modo síncrono e streaming SSE.</summary>
public interface IAskHelperPipeline : IPipeline<string, string>, IStreamingPipeline<string>;
