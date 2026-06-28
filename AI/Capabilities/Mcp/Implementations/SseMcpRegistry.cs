using Domain.Capabilities;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using System.Collections.Concurrent;

namespace AI.Capabilities.Mcp.Implementations;

/// <summary>
/// Registry de MCPs via transport HTTP (SSE ou Streamable HTTP, conforme
/// negociação automática do <see cref="HttpClientTransport"/>).
/// O endpoint é a URL absoluta do servidor MCP.
/// </summary>
public sealed class SseMcpRegistry(ILogger<SseMcpRegistry> logger) : McpRegistryBase
{
    private readonly ConcurrentDictionary<string, McpEntry> _entries = new();

    public override async Task RegisterAsync(McpDescriptor descriptor, CancellationToken ct = default)
    {
        if (descriptor.Transport is not (McpTransport.Sse or McpTransport.Http))
            throw new ArgumentException($"SseMcpRegistry only accepts Sse/Http descriptors (got {descriptor.Transport})");

        if (_entries.ContainsKey(descriptor.Name))
        {
            logger.LogDebug("MCP {Name} already registered, skipping.", descriptor.Name);
            return;
        }

        var transport = new HttpClientTransport(new HttpClientTransportOptions
        {
            Name = descriptor.Name,
            Endpoint = new Uri(descriptor.Endpoint.Value),
        });

        var client = await McpClient.CreateAsync(transport, cancellationToken: ct);
        var tools = await client.ListToolsAsync(cancellationToken: ct);

        _entries[descriptor.Name] = new McpEntry(client, tools.Cast<AITool>().ToList(), descriptor);
        logger.LogInformation("Registered HTTP MCP {Name} with {Count} tools.", descriptor.Name, tools.Count);
    }

    public override Task<IReadOnlyList<McpDescriptor>> GetConnectedMcpsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<McpDescriptor>>(
            _entries.Values.Select(e => e.Descriptor).ToList().AsReadOnly());

    public override Task<IReadOnlyList<AITool>> GetAllToolsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<AITool>>(
            _entries.Values.SelectMany(e => e.Tools).ToList().AsReadOnly());

    public override async Task DisconnectAsync(string mcpName, CancellationToken ct = default)
    {
        if (_entries.TryRemove(mcpName, out var entry))
        {
            await entry.Client.DisposeAsync();
            logger.LogInformation("Disconnected MCP {Name}.", mcpName);
        }
    }

    public override Task<bool> IsConnectedAsync(string mcpName, CancellationToken ct = default)
        => Task.FromResult(_entries.ContainsKey(mcpName));

    public override async ValueTask DisposeAsync()
    {
        foreach (var entry in _entries.Values)
            await entry.Client.DisposeAsync();
        _entries.Clear();
    }

    private sealed record McpEntry(McpClient Client, IReadOnlyList<AITool> Tools, McpDescriptor Descriptor);
}
