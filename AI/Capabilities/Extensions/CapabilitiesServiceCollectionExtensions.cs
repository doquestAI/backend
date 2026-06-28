using AI.Capabilities.Mcp;
using AI.Capabilities.Mcp.Implementations;
using AI.Capabilities.Plugins;
using Domain.Interfaces.Capabilities;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Capabilities.Extensions;

public static class CapabilitiesServiceCollectionExtensions
{
    /// <summary>Registra os componentes de Capabilities (Plugins, MCPs, Provider).</summary>
    public static IServiceCollection AddAgentCapabilities(this IServiceCollection services)
    {
        // Plugin registry (interface + concreto)
        services.AddSingleton<PluginRegistry>();
        services.AddSingleton<IPluginRegistry>(sp => sp.GetRequiredService<PluginRegistry>());
        services.AddSingleton<IPluginRegistrar>(sp => sp.GetRequiredService<PluginRegistry>());

        // MCP registries específicos por transport + connector que roteia
        services.AddSingleton<StdIoMcpRegistry>();
        services.AddSingleton<SseMcpRegistry>();
        services.AddSingleton<IEnumerable<McpRegistryBase>>(sp => new McpRegistryBase[]
        {
            sp.GetRequiredService<StdIoMcpRegistry>(),
            sp.GetRequiredService<SseMcpRegistry>(),
        });
        services.AddSingleton<IMcpConnector, McpConnector>();

        // Provider público (Application) — agrega plugins + MCPs em uma visão única
        services.AddSingleton<ICapabilitiesProvider, CapabilitiesProvider>();

        return services;
    }
}
