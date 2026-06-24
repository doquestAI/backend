namespace Application.Pipelines.Abstractions;

/// <summary>Lançada quando um step <c>Validate</c> falha. Convertida em 400 pela camada de aplicação.</summary>
public sealed class PipelineValidationException(string message) : Exception(message);
