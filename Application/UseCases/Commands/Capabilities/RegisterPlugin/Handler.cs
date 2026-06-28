using Domain.Interfaces.Capabilities;
using Flunt.Notifications;
using MediatR;

namespace Application.UseCases.Commands.Capabilities.RegisterPlugin;

internal sealed class Handler(IPluginRegistrar registrar)
    : IRequestHandler<Request, Response>
{
    public Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Task.FromResult(new Response(
                StatusCode: 400,
                Message: "Plugin name is required.",
                Notifications: [new Notification(nameof(request.Name), "Plugin name cannot be empty")]));

        registrar.RegisterEmpty(request.Name, request.Description, request.Enabled);
        return Task.FromResult(new Response(StatusCode: 200, PluginName: request.Name));
    }
}
