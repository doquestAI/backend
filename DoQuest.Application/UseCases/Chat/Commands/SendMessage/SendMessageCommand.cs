using DoQuest.Application.Common;
using MediatR;
using Microsoft.Extensions.AI;

namespace DoQuest.Application.UseCases.Chat.Commands.SendMessage;

public sealed record SendMessageCommand(
    string FirebaseUid,
    string Message,
    Guid? VestibularId,
    IEnumerable<ChatMessage> ConversationHistory
) : IRequest<Result<SendMessageResponse>>;
