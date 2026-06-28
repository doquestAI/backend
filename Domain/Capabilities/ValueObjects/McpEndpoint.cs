using Domain.Common;
using Flunt.Validations;

namespace Domain.Capabilities.ValueObjects;

/// <summary>
/// Endpoint de um servidor MCP. Para transports StdIo o "endpoint" é o comando
/// + argumentos (ex: <c>npx -y @modelcontextprotocol/server-github</c>);
/// para Sse/Http é uma URL válida.
/// </summary>
public sealed class McpEndpoint : ValueObject
{
    public string Value { get; private set; } = string.Empty;
    public McpTransport Transport { get; private set; }

    public McpEndpoint(string value, McpTransport transport)
    {
        AddNotifications(
            new Contract<McpEndpoint>()
                .IsNotNullOrEmpty(value, nameof(McpEndpoint), "MCP endpoint cannot be empty")
                .IsLowerOrEqualsThan(value?.Length ?? 0, 1000, nameof(McpEndpoint),
                    "MCP endpoint cannot exceed 1000 characters"));

        if (transport is McpTransport.Sse or McpTransport.Http
            && !Uri.TryCreate(value, UriKind.Absolute, out _))
        {
            AddNotification(nameof(McpEndpoint), $"MCP endpoint must be a valid absolute URI for {transport} transport");
        }

        if (IsValid)
        {
            Value = value!;
            Transport = transport;
        }
    }

    public override string ToString() => $"{Transport}://{Value}";
}
