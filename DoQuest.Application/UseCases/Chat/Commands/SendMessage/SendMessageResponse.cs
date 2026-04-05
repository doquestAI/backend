namespace DoQuest.Application.UseCases.Chat.Commands.SendMessage;

public sealed record SendMessageResponse(
    string Reply,
    int SourceChunksUsed,
    int RemainingMessages);
