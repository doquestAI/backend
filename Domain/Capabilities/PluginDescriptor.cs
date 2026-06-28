namespace Domain.Capabilities;

/// <summary>
/// Descreve um plugin disponível para o Agent.
/// Não conhece como o plugin é executado — apenas o que ele é.
/// </summary>
internal sealed class PluginDescriptor
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required IReadOnlyList<FunctionCallDescriptor> Functions { get; init; }
    public bool IsEnabled { get; init; } = true;
}
