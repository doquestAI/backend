namespace Application.UseCases.Chat.Commands.CreateSession;

internal sealed record CreateChatSessionResponse(
    Guid SessionId,
    string Title,
    DateTime CreatedAt);
