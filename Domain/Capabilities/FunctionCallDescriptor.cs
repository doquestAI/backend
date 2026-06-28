namespace Domain.Capabilities;

/// <summary>
/// Descreve uma function call que o Agent pode invocar.
/// Wrapper semântico sobre <c>AIFunction</c>/<c>AIFunctionMetadata</c> do MAF,
/// mantendo o Domain livre da dependência do framework.
/// </summary>
public sealed class FunctionCallDescriptor
{
    public required string PluginName { get; init; }
    public required string FunctionName { get; init; }
    public required string Description { get; init; }
    public required IReadOnlyList<ParameterDescriptor> Parameters { get; init; }
    public string ReturnDescription { get; init; } = string.Empty;
    public FunctionCallSource Source { get; init; }
}
