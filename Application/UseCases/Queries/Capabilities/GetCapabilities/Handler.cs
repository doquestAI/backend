using Application.Dtos.Capabilities;
using Domain.Interfaces.Capabilities;
using MediatR;

namespace Application.UseCases.Queries.Capabilities.GetCapabilities;

internal sealed class Handler(ICapabilitiesProvider provider)
    : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var plugins = provider.GetPlugins();
        var mcps = await provider.GetMcpsAsync(cancellationToken);
        var functions = await provider.GetFunctionsAsync(cancellationToken);

        var payload = new CapabilitiesResponse(
            Plugins: plugins.Select(p => new PluginDto(
                p.Name, p.Description, p.Functions.Count, p.IsEnabled)).ToList().AsReadOnly(),
            Mcps: mcps.Select(m => new McpDto(
                m.Name, m.Transport.ToString(), m.Endpoint.Value,
                m.ExposedFunctions.Count, m.IsConnected)).ToList().AsReadOnly(),
            Functions: functions.Select(f => new FunctionDto(
                f.PluginName, f.FunctionName, f.Description, f.Source.ToString()))
                .ToList().AsReadOnly());

        return new Response(StatusCode: 200, Capabilities: payload);
    }
}
