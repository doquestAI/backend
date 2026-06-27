using Domain.Agents.Enem;
using Domain.Exceptions;
using Domain.Interfaces.Pipelines.Enem;
using MediatR;

namespace Application.UseCases.Commands.Enem.ExplainTopic;

internal sealed class Handler(IExplainTopicPipeline pipeline)
    : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        try
        {
            var explanation = await pipeline.RunAsync(
                new ExplainRequest(request.Topic, request.Area),
                cancellationToken);

            return new Response(StatusCode: 200, Explanation: explanation);
        }
        catch (PipelineValidationException ex)
        {
            return new Response(StatusCode: 400, Message: ex.Message);
        }
    }
}
