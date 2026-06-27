using Domain.Exceptions;
using Domain.Interfaces.Pipelines.Enem;
using MediatR;

namespace Application.UseCases.Commands.Enem.AskHelper;

internal sealed class Handler(IAskHelperPipeline pipeline)
    : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        try
        {
            var answer = await pipeline.RunAsync(request.Question, cancellationToken);
            return new Response(StatusCode: 200, Answer: answer);
        }
        catch (PipelineValidationException ex)
        {
            return new Response(StatusCode: 400, Message: ex.Message);
        }
    }
}
