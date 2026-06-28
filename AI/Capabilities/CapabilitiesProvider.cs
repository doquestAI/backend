using AI.Capabilities.Plugins;
using Domain.Capabilities;
using Domain.Interfaces.Capabilities;

namespace AI.Capabilities;

/// <summary>
/// Implementação de <see cref="ICapabilitiesProvider"/>: agrega plugins do
/// <see cref="PluginRegistry"/> + MCPs do <see cref="IMcpConnector"/> em uma
/// visão consolidada para uso pelo Application.
/// </summary>
internal sealed class CapabilitiesProvider(
    PluginRegistry pluginRegistry,
    IMcpConnector mcpConnector) : ICapabilitiesProvider
{
    public IReadOnlyList<PluginDescriptor> GetPlugins() => pluginRegistry.GetEnabledPlugins();

    public Task<IReadOnlyList<McpDescriptor>> GetMcpsAsync(CancellationToken ct = default) =>
        mcpConnector.GetConnectedAsync(ct);

    public async Task<IReadOnlyList<FunctionCallDescriptor>> GetFunctionsAsync(CancellationToken ct = default)
    {
        var plugins = pluginRegistry.GetEnabledPlugins();
        var mcps = await mcpConnector.GetConnectedAsync(ct);
        return plugins.SelectMany(p => p.Functions)
            .Concat(mcps.SelectMany(m => m.ExposedFunctions))
            .ToList().AsReadOnly();
    }
}
