using Domain.Capabilities;
using Microsoft.Extensions.AI;

namespace AI.Capabilities.Mcp;

/// <summary>
/// Classe base abstrata para registries de servidores MCP.
/// Implementações concretas (<see cref="Implementations.StdIoMcpRegistry"/>,
/// <see cref="Implementations.SseMcpRegistry"/>) gerenciam o ciclo de vida do
/// <c>IMcpClient</c> e expõem suas tools como <see cref="AITool"/> ao
/// <c>ChatClientAgent</c> do MAF.
/// </summary>
public abstract class McpRegistryBase : IAsyncDisposable
{
    public abstract Task RegisterAsync(McpDescriptor descriptor, CancellationToken ct = default);

    public abstract Task<IReadOnlyList<McpDescriptor>> GetConnectedMcpsAsync(
        CancellationToken ct = default);

    public abstract Task<IReadOnlyList<AITool>> GetAllToolsAsync(CancellationToken ct = default);

    public abstract Task DisconnectAsync(string mcpName, CancellationToken ct = default);

    public abstract Task<bool> IsConnectedAsync(string mcpName, CancellationToken ct = default);

    public abstract ValueTask DisposeAsync();
}
