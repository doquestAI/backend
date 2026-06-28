using Domain.Capabilities.ValueObjects;

namespace Domain.Capabilities;

/// <summary>Descreve um servidor MCP conectado ao Agent.</summary>
public sealed class McpDescriptor
{
    public required string Name { get; init; }
    public required McpEndpoint Endpoint { get; init; }
    public required IReadOnlyList<FunctionCallDescriptor> ExposedFunctions { get; init; }
    public bool IsConnected { get; init; }

    public McpTransport Transport => Endpoint.Transport;
}
