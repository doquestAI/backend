using MediatR;

namespace Application.UseCases.Commands.Chat.Session.Create;

internal record Request(
    Guid ContextUserId,
    string? Title = null,
    string? Description = null) : IRequest<Response>;
