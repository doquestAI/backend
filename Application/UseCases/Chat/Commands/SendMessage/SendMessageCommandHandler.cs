using Application.Common;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services.AI;
using MediatR;

namespace Application.UseCases.Chat.Commands.SendMessage;

internal sealed class SendMessageCommandHandler(
    IUserRepository userRepository,
    IChatSessionRepository chatSessionRepository,
    IChatAgentService chatAgentService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<SendMessageCommand, Result<SendMessageResponse>>
{
    public async Task<Result<SendMessageResponse>> Handle(
        SendMessageCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Load user with plan
        var user = await userRepository.GetByFirebaseUidAsync(request.FirebaseUid, cancellationToken);
        if (user is null)
            return Result.Failure<SendMessageResponse>("User", "Usuário não encontrado");

        // 2. Enforce plan message limit
        user.ConsumeMessage();
        if (!user.IsValid)
            return Result.Failure<SendMessageResponse>(user.Notifications);

        // 3. Verify session belongs to user
        var session = await chatSessionRepository.GetWithMessagesAsync(request.SessionId,
             messageLimit: 0, cancellationToken);

        if (session is null || session.UserId != user.Id)
            return Result.Failure<SendMessageResponse>("Session", "Sessão não encontrada");

        // 4. Run RAG pipeline via chat agent service
        var agentResponse = await chatAgentService.ProcessMessageAsync(
            request.SessionId,
            request.Message,
            request.VestibularId,
            cancellationToken);

        // 5. Persist updated daily counter
        userRepository.Update(user);
        await unitOfWork.CommitAsync(cancellationToken);

        var remaining = user.Plan.DailyMessageLimit.IsUnlimited
            ? int.MaxValue
            : Math.Max(0, user.Plan.DailyMessageLimit.Value - user.DailyMessageCount);

        return Result.Success(new SendMessageResponse(agentResponse.Reply, agentResponse.SourceChunksUsed, remaining));
    }
}
