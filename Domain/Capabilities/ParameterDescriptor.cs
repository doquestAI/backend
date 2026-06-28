namespace Domain.Capabilities;

internal sealed class ParameterDescriptor
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Type { get; init; }
    public bool IsRequired { get; init; }
    public string? DefaultValue { get; init; }
}
