using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Queries.Chat.Session.GetWithMessages;

internal class Handler(IChatSessionRepository sessionRepository) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var session = await sessionRepository.GetByIdWithMessagesAsync(request.SessionId, cancellationToken);

        if (session is null)
            return new Response(StatusCode: 404, Message: "Chat session not found");

        var messages = session.Messages
            .Select(m => new ChatMessageDto(m.Id, m.Role, m.Content, m.CreatedDate ?? DateTime.UtcNow))
            .ToList();

        return new Response(
            StatusCode: 200,
            SessionId: session.Id,
            Title: session.Title,
            Description: session.Description,
            EndedAt: session.EndedAt,
            Messages: messages);
    }
}
