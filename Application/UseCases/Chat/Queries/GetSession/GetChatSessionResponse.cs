using Domain.Enums;

namespace Application.UseCases.Chat.Queries.GetSession;

internal sealed record ChatMessageDto(
    Guid Id,
    ChatMessageRole Role,
    string Content,
    DateTime CreatedAt);

internal sealed record GetChatSessionResponse(
    Guid Id,
    string Title,
    Guid? VestibularId,
    DateTime? LastMessageAt,
    DateTime CreatedAt,
    IReadOnlyList<ChatMessageDto> Messages);
