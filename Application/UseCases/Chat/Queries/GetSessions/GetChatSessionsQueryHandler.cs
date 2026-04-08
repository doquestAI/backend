using Application.Common;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Chat.Queries.GetSessions;

internal sealed class GetChatSessionsQueryHandler(
    IUserRepository userRepository,
    IChatSessionRepository chatSessionRepository)
    : IRequestHandler<GetChatSessionsQuery, Result<GetChatSessionsResponse>>
{
    public async Task<Result<GetChatSessionsResponse>> Handle(
        GetChatSessionsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByFirebaseUidAsync(request.FirebaseUid, cancellationToken);
        if (user is null)
            return Result.Failure<GetChatSessionsResponse>("User", "Usuário não encontrado");

        var sessions = await chatSessionRepository.GetByUserIdAsync(
            user.Id, request.Page, request.PageSize, cancellationToken);

        var summaries = sessions
            .Select(s => new ChatSessionSummary(s.Id, s.Title, s.VestibularId, s.LastMessageAt, s.CreatedAt))
            .ToList();

        return Result.Success(new GetChatSessionsResponse(summaries, request.Page, request.PageSize));
    }
}
