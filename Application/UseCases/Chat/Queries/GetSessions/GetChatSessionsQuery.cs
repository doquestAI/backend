using Application.Common;
using MediatR;

namespace Application.UseCases.Chat.Queries.GetSessions;

internal sealed record GetChatSessionsQuery(
    string FirebaseUid,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<GetChatSessionsResponse>>;
