using MediatR;

namespace Application.UseCases.Commands.Chat.Message.Add;

internal record Request(
    Guid SessionId,
    string Role,
    string Content) : IRequest<Response>;
