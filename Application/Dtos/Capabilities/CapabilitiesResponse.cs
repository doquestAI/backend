namespace Application.Dtos.Capabilities;

internal sealed record PluginDto(string Name, string Description, int FunctionCount, bool Enabled);
internal sealed record McpDto(string Name, string Transport, string Endpoint, int FunctionCount, bool Connected);
internal sealed record FunctionDto(string PluginName, string FunctionName, string Description, string Source);

internal sealed record CapabilitiesResponse(
    IReadOnlyList<PluginDto> Plugins,
    IReadOnlyList<McpDto> Mcps,
    IReadOnlyList<FunctionDto> Functions);
