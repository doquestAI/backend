using Domain.Agents.Enem;
using Domain.Exceptions;
using Domain.Interfaces.Pipelines.Enem;
using MediatR;

namespace Application.UseCases.Commands.Enem.GenerateQuestion;

internal sealed class Handler(IGenerateQuestionPipeline pipeline)
    : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        try
        {
            var question = await pipeline.RunAsync(
                new QuestionRequest(request.Topic, request.Area, request.Difficulty),
                cancellationToken);

            return new Response(
                StatusCode: 200,
                Statement: question.Statement,
                Options: question.Options,
                CorrectKey: question.CorrectKey,
                Explanation: question.Explanation,
                Area: question.Area);
        }
        catch (PipelineValidationException ex)
        {
            return new Response(StatusCode: 400, Message: ex.Message);
        }
    }
}
