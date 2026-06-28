namespace Application.Dtos.Capabilities;

public sealed record PluginDto(string Name, string Description, int FunctionCount, bool Enabled);
public sealed record McpDto(string Name, string Transport, string Endpoint, int FunctionCount, bool Connected);
public sealed record FunctionDto(string PluginName, string FunctionName, string Description, string Source);

public sealed record CapabilitiesResponse(
    IReadOnlyList<PluginDto> Plugins,
    IReadOnlyList<McpDto> Mcps,
    IReadOnlyList<FunctionDto> Functions);
