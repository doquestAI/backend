namespace Application.UseCases.Chat.Commands.SendMessage;

internal sealed record SendMessageResponse(
    string Reply,
    int SourceChunksUsed,
    int RemainingMessages);
