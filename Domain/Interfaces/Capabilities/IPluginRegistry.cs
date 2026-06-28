using Domain.Capabilities;

namespace Domain.Interfaces.Capabilities;

/// <summary>
/// Abstração para o catálogo de plugins (Function Tools) registrados no Agent.
/// Implementação concreta vive na camada AI sobre <c>AIFunction</c> do MAF.
/// </summary>
internal interface IPluginRegistry
{
    IReadOnlyList<PluginDescriptor> GetEnabledPlugins();
}
