using Domain.Capabilities;

namespace Domain.Interfaces.Capabilities;

/// <summary>
/// Visão agregada das capabilities (plugins + MCPs + functions) disponíveis ao Agent.
/// Implementado na AI por <c>CapabilitiesProvider</c>.
/// </summary>
internal interface ICapabilitiesProvider
{
    IReadOnlyList<PluginDescriptor> GetPlugins();
    Task<IReadOnlyList<McpDescriptor>> GetMcpsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<FunctionCallDescriptor>> GetFunctionsAsync(CancellationToken ct = default);
}
