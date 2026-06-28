using Domain.Capabilities;
using Domain.Capabilities.ValueObjects;
using Domain.Interfaces.Capabilities;
using MediatR;

namespace Application.UseCases.Commands.Capabilities.ConnectMcp;

internal sealed class Handler(IMcpConnector connector)
    : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var endpoint = new McpEndpoint(request.Endpoint, request.Transport);
        if (!endpoint.IsValid)
            return new Response(
                StatusCode: 400,
                Message: "Invalid MCP endpoint.",
                Notifications: endpoint.Notifications.ToList());

        var descriptor = new McpDescriptor
        {
            Name = request.Name,
            Endpoint = endpoint,
            ExposedFunctions = [],
            IsConnected = true,
        };

        await connector.RegisterAsync(descriptor, cancellationToken);
        return new Response(StatusCode: 200, McpName: request.Name);
    }
}
