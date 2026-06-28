using Domain.Interfaces.Pipelines.Enem;
using MediatR;

namespace Application.UseCases.Commands.Enem.AskHelper;

internal sealed class Handler(IAskHelperPipeline pipeline)
    : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var result = await pipeline.RunAsync(request.Question, cancellationToken);

        if (!result.IsValid)
            return new Response(
                StatusCode: 400,
                Message: string.Join("; ", result.Notifications.Select(n => n.Message)),
                Notifications: result.Notifications.ToList());

        return new Response(StatusCode: 200, Answer: result.Value);
    }
}
