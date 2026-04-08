using Application.Common;
using MediatR;

namespace Application.UseCases.Chat.Commands.CreateSession;

internal sealed record CreateChatSessionCommand(
    string FirebaseUid,
    Guid? VestibularId,
    string? InitialMessage) : IRequest<Result<CreateChatSessionResponse>>;
