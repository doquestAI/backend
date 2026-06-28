using Domain.Capabilities;
using Domain.Interfaces.Capabilities;
using Microsoft.Extensions.AI;
using System.Collections.Concurrent;

namespace AI.Capabilities.Plugins;

/// <summary>
/// Catálogo de plugins (conjuntos de <see cref="AIFunction"/>) registrados estaticamente
/// no Agent. Para MAF, plugins são apenas agrupamentos lógicos de Function Tools criadas
/// via <c>AIFunctionFactory.Create(...)</c>.
/// </summary>
public sealed class PluginRegistry : IPluginRegistry, IPluginRegistrar
{
    private readonly ConcurrentDictionary<string, PluginEntry> _plugins = new();

    public void RegisterEmpty(string name, string description, bool enabled = true) =>
        Register(name, description, [], enabled);

    public void Register(string pluginName, string description, IEnumerable<AIFunction> functions, bool enabled = true)
    {
        var fnList = functions.ToList();
        var entry = new PluginEntry(
            new PluginDescriptor
            {
                Name = pluginName,
                Description = description,
                Functions = fnList.Select(fn => ToDescriptor(pluginName, fn)).ToList().AsReadOnly(),
                IsEnabled = enabled,
            },
            fnList);

        _plugins[pluginName] = entry;
    }

    public IReadOnlyList<PluginDescriptor> GetEnabledPlugins() =>
        _plugins.Values.Where(e => e.Descriptor.IsEnabled)
            .Select(e => e.Descriptor)
            .ToList().AsReadOnly();

    public IReadOnlyList<AIFunction> GetAllFunctions() =>
        _plugins.Values.Where(e => e.Descriptor.IsEnabled)
            .SelectMany(e => e.Functions)
            .ToList().AsReadOnly();

    private static FunctionCallDescriptor ToDescriptor(string pluginName, AIFunction fn) =>
        new()
        {
            PluginName = pluginName,
            FunctionName = fn.Name,
            Description = fn.Description ?? string.Empty,
            Parameters = [],
            Source = FunctionCallSource.Plugin,
        };

    private sealed record PluginEntry(PluginDescriptor Descriptor, IReadOnlyList<AIFunction> Functions);
}
