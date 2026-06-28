using Domain.Agents.Enem;
using Domain.Interfaces.Pipelines.Enem;
using MediatR;

namespace Application.UseCases.Commands.Enem.ExplainTopic;

internal sealed class Handler(IExplainTopicPipeline pipeline)
    : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var result = await pipeline.RunAsync(
            new ExplainRequest(request.Topic, request.Area),
            cancellationToken);

        if (!result.IsValid)
            return new Response(
                StatusCode: 400,
                Message: string.Join("; ", result.Notifications.Select(n => n.Message)),
                Notifications: result.Notifications.ToList());

        return new Response(StatusCode: 200, Explanation: result.Value);
    }
}
