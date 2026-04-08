namespace Application.UseCases.Chat.Queries.GetSessions;

internal sealed record ChatSessionSummary(
    Guid Id,
    string Title,
    Guid? VestibularId,
    DateTime? LastMessageAt,
    DateTime CreatedAt);

internal sealed record GetChatSessionsResponse(
    IReadOnlyList<ChatSessionSummary> Sessions,
    int Page,
    int PageSize);
