using AI.Capabilities.Plugins;
using Domain.Interfaces.Capabilities;
using Microsoft.Extensions.DependencyInjection;

namespace AI.Capabilities.Extensions;

internal static class CapabilitiesServiceCollectionExtensions
{
    /// <summary>Registra os componentes de Capabilities (Plugins, MCPs, Provider).</summary>
    internal static IServiceCollection AddAgentCapabilities(this IServiceCollection services)
    {
        // Plugin registry (interface + concreto)
        services.AddSingleton<PluginRegistry>();
        services.AddSingleton<IPluginRegistry>(sp => sp.GetRequiredService<PluginRegistry>());
        services.AddSingleton<IPluginRegistrar>(sp => sp.GetRequiredService<PluginRegistry>());

        // Provider público (Application) — agrega plugins + MCPs em uma visão única
        services.AddSingleton<ICapabilitiesProvider, CapabilitiesProvider>();

        return services;
    }
}
