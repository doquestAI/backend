using Domain.Agents.Enem;
using Domain.Interfaces.Pipelines.Enem;
using MediatR;

namespace Application.UseCases.Commands.Enem.GenerateQuestion;

internal sealed class Handler(IGenerateQuestionPipeline pipeline)
    : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var result = await pipeline.RunAsync(
            new QuestionRequest(request.Topic, request.Area, request.Difficulty),
            cancellationToken);

        if (!result.IsValid)
            return new Response(
                StatusCode: 400,
                Message: string.Join("; ", result.Notifications.Select(n => n.Message)),
                Notifications: result.Notifications.ToList());

        var question = result.Value!;
        return new Response(
            StatusCode: 200,
            Statement: question.Statement,
            Options: question.Options,
            CorrectKey: question.CorrectKey,
            Explanation: question.Explanation,
            Area: question.Area);
    }
}
