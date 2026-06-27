namespace Domain.Exceptions;

/// <summary>
/// Lançada quando um step <c>Validate</c> de pipeline falha.
/// Convertida em 400 BadRequest pelos Handlers da camada Application.
/// </summary>
public sealed class PipelineValidationException(string message) : Exception(message);
