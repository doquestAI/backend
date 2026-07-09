using Domain.Entities.Core.Chat;
using Domain.Interfaces.Repositories;
using Flunt.Notifications;
using Flunt.Validations;
using MediatR;

namespace Application.UseCases.Commands.Chat.Message.Add;

internal class Handler(
    IChatMessageRepository messageRepository,
    IDbCommit dbCommit) : IRequestHandler<Request, Response>
{
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var contract = new Contract<Notifiable<Notification>>()
            .Requires()
            .IsNotEmpty(request.SessionId, "SessionId", "SessionId is required")
            .IsNotNull(request.Role, "Role", "Role is required")
            .IsNotNull(request.Content, "Content", "Content is required")
            .IsGreaterThan(request.Content?.Length ?? 0, 0, "Content", "Content cannot be empty");

        if (!contract.IsValid)
            return new Response(
                StatusCode: 400,
                Message: "Request invalid",
                Notifications: contract.Notifications.ToList());

        var message = new ChatMessage(request.SessionId, request.Role, request.Content);

        await messageRepository.CreateAsync(message, cancellationToken);
        await dbCommit.Commit(cancellationToken);

        return new Response(
            StatusCode: 201,
            Message: "Message added successfully",
            MessageId: message.Id);
    }
}
