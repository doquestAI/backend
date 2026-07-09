using MediatR;

namespace Application.UseCases.Queries.Chat.Session.GetWithMessages;

internal record Request(Guid SessionId) : IRequest<Response>;
