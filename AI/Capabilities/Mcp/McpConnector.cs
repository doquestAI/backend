using AI.Capabilities.Mcp.Implementations;
using Domain.Capabilities;
using Domain.Interfaces.Capabilities;

namespace AI.Capabilities.Mcp;

/// <summary>
/// Roteia <see cref="McpDescriptor"/> para o <see cref="McpRegistryBase"/> correto
/// conforme o <see cref="McpTransport"/>. Implementa a abstração
/// <see cref="IMcpConnector"/> consumida pela Application.
/// </summary>
public sealed class McpConnector(
    StdIoMcpRegistry stdio,
    SseMcpRegistry http) : IMcpConnector
{
    public Task RegisterAsync(McpDescriptor descriptor, CancellationToken ct = default) =>
        Pick(descriptor.Transport).RegisterAsync(descriptor, ct);

    public async Task<IReadOnlyList<McpDescriptor>> GetConnectedAsync(CancellationToken ct = default)
    {
        var stdioMcps = await stdio.GetConnectedMcpsAsync(ct);
        var httpMcps = await http.GetConnectedMcpsAsync(ct);
        return stdioMcps.Concat(httpMcps).ToList().AsReadOnly();
    }

    public async Task DisconnectAsync(string mcpName, CancellationToken ct = default)
    {
        await stdio.DisconnectAsync(mcpName, ct);
        await http.DisconnectAsync(mcpName, ct);
    }

    private McpRegistryBase Pick(McpTransport transport) => transport switch
    {
        McpTransport.StdIo => stdio,
        McpTransport.Sse or McpTransport.Http => http,
        _ => throw new ArgumentOutOfRangeException(nameof(transport), transport, "Unknown MCP transport"),
    };
}
