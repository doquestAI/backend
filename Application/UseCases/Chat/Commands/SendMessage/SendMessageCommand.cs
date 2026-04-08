using Application.Common;
using MediatR;

namespace Application.UseCases.Chat.Commands.SendMessage;

internal sealed record SendMessageCommand(
    string FirebaseUid,
    Guid SessionId,
    string Message,
    Guid? VestibularId = null) : IRequest<Result<SendMessageResponse>>;
