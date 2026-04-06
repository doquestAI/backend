using Application.Common;
using MediatR;
using Microsoft.Extensions.AI;

namespace Application.UseCases.Chat.Commands.SendMessage;

internal sealed record SendMessageCommand(
    string FirebaseUid,
    string Message,
    Guid? VestibularId,
    IEnumerable<ChatMessage> ConversationHistory
) : IRequest<Result<SendMessageResponse>>;
