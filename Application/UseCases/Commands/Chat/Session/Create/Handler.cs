using Domain.Entities.Core.Chat;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Commands.Chat.Session.Create;

internal class Handler(
    IChatSessionRepository sessionRepository,
    IDbCommit dbCommit) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var session = new ChatSession(request.ContextUserId, request.Title, request.Description);

        await sessionRepository.CreateAsync(session, cancellationToken);
        await dbCommit.Commit(cancellationToken);

        return new Response(
            StatusCode: 201,
            Message: "Chat session created successfully",
            SessionId: session.Id);
    }
}
