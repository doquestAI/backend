using Application.Common;
using MediatR;

namespace Application.UseCases.Chat.Queries.GetSession;

internal sealed record GetChatSessionQuery(
    string FirebaseUid,
    Guid SessionId,
    int MessageLimit = 50) : IRequest<Result<GetChatSessionResponse>>;
