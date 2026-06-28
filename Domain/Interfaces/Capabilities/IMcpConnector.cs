using Domain.Capabilities;

namespace Domain.Interfaces.Capabilities;

/// <summary>
/// Abstração para conectar/desconectar servidores MCP. A Application invoca isso
/// sem conhecer transporte (StdIo, SSE, HTTP). A implementação concreta seleciona
/// o registry correto (<c>StdIoMcpRegistry</c>, <c>SseMcpRegistry</c>) na camada AI.
/// </summary>
internal interface IMcpConnector
{
    Task RegisterAsync(McpDescriptor descriptor, CancellationToken ct = default);

    Task<IReadOnlyList<McpDescriptor>> GetConnectedAsync(CancellationToken ct = default);

    Task DisconnectAsync(string mcpName, CancellationToken ct = default);
}
