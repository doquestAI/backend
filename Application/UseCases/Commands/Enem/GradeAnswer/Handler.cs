using Domain.Agents.Enem;
using Domain.Interfaces.Pipelines.Enem;
using MediatR;

namespace Application.UseCases.Commands.Enem.GradeAnswer;

internal sealed class Handler(IGradeAnswerPipeline pipeline)
    : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var result = await pipeline.RunAsync(
            new FeedbackRequest(request.Question, request.StudentAnswer, request.CorrectAnswer, request.Area),
            cancellationToken);

        if (!result.IsValid)
            return new Response(
                StatusCode: 400,
                Message: string.Join("; ", result.Notifications.Select(n => n.Message)),
                Notifications: result.Notifications.ToList());

        var feedback = result.Value!;
        return new Response(
            StatusCode: 200,
            IsCorrect: feedback.IsCorrect,
            Explanation: feedback.Explanation,
            Score: feedback.Score);
    }
}
