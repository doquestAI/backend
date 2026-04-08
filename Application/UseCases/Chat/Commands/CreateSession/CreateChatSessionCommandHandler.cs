using Application.Common;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Chat.Commands.CreateSession;

internal sealed class CreateChatSessionCommandHandler(
    IUserRepository userRepository,
    IChatSessionRepository chatSessionRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateChatSessionCommand, Result<CreateChatSessionResponse>>
{
    public async Task<Result<CreateChatSessionResponse>> Handle(
        CreateChatSessionCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByFirebaseUidAsync(request.FirebaseUid, cancellationToken);
        if (user is null)
            return Result.Failure<CreateChatSessionResponse>("User", "Usuário não encontrado");

        var title = string.IsNullOrWhiteSpace(request.InitialMessage)
            ? "Nova conversa"
            : TruncateTitle(request.InitialMessage);

        var session = ChatSession.Create(user.Id, title, request.VestibularId);
        if (!session.IsValid)
            return Result.Failure<CreateChatSessionResponse>(session.Notifications);

        await chatSessionRepository.CreateAsync(session, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new CreateChatSessionResponse(session.Id, session.Title, session.CreatedAt));
    }

    private static string TruncateTitle(string message)
    {
        const int maxLength = 80;
        var trimmed = message.Trim();
        return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength] + "...";
    }
}
