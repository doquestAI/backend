using Application.Common;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Chat.Queries.GetSession;

internal sealed class GetChatSessionQueryHandler(
    IUserRepository userRepository,
    IChatSessionRepository chatSessionRepository)
    : IRequestHandler<GetChatSessionQuery, Result<GetChatSessionResponse>>
{
    public async Task<Result<GetChatSessionResponse>> Handle(
        GetChatSessionQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByFirebaseUidAsync(request.FirebaseUid, cancellationToken);
        if (user is null)
            return Result.Failure<GetChatSessionResponse>("User", "Usuário não encontrado");

        var session = await chatSessionRepository.GetWithMessagesAsync(
            request.SessionId, request.MessageLimit, cancellationToken);

        if (session is null || session.UserId != user.Id)
            return Result.Failure<GetChatSessionResponse>("Session", "Sessão não encontrada");

        var messages = session.Messages
            .OrderBy(m => m.CreatedAt)
            .Select(m => new ChatMessageDto(m.Id, m.Role, m.Content, m.CreatedAt))
            .ToList();

        return Result.Success(new GetChatSessionResponse(
            session.Id, session.Title, session.VestibularId,
            session.LastMessageAt, session.CreatedAt, messages));
    }
}
