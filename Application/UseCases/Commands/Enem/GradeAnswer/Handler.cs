using Application.Pipelines.Abstractions;
using Application.Pipelines.Enem.Abstractions;
using Domain.Agents.Enem;
using MediatR;

namespace Application.UseCases.Commands.Enem.GradeAnswer;

internal sealed class Handler(IGradeAnswerPipeline pipeline)
    : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        try
        {
            var feedback = await pipeline.RunAsync(
                new FeedbackRequest(request.Question, request.StudentAnswer, request.CorrectAnswer, request.Area),
                cancellationToken);

            return new Response(
                StatusCode: 200,
                IsCorrect: feedback.IsCorrect,
                Explanation: feedback.Explanation,
                Score: feedback.Score);
        }
        catch (PipelineValidationException ex)
        {
            return new Response(StatusCode: 400, Message: ex.Message);
        }
    }
}
