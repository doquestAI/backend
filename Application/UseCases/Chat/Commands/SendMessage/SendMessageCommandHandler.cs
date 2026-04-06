using Application.Common;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using MediatR;

namespace Application.UseCases.Chat.Commands.SendMessage;

internal sealed class SendMessageCommandHandler(
    IUserRepository userRepository,
    IDocumentRepository documentRepository,
    IEmbeddingService embeddingService,
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

        // 2. Enforce plan message limit (domain rule — no exception)
        user.ConsumeMessage();
        if (!user.IsValid)
            return Result.Failure<SendMessageResponse>(user.Notifications);

        // 3. Generate query embedding
        var queryEmbedding = await embeddingService.GenerateAsync(request.Message, cancellationToken);

        // 4. Retrieve semantically relevant chunks from pgvector
        var chunks = await documentRepository.SearchSimilarChunksAsync(
            queryEmbedding,
            topK: 5,
            request.VestibularId,
            cancellationToken);

        // 5. Build RAG context string
        var ragContext = chunks.Count > 0
            ? string.Join("\n\n---\n\n", chunks.Select((c, i) => $"[Fonte {i + 1}] {c.Text.Value}"))
            : string.Empty;

        // 6. Call MAF chat agent with context
        var reply = await chatAgentService.ChatAsync(
            request.Message,
            ragContext,
            request.ConversationHistory,
            cancellationToken);

        // 7. Persist updated daily counter
        userRepository.Update(user);
        await unitOfWork.CommitAsync(cancellationToken);

        var remaining = user.Plan.DailyMessageLimit.IsUnlimited
            ? int.MaxValue
            : Math.Max(0, user.Plan.DailyMessageLimit.Value - user.DailyMessageCount);

        return Result.Success(new SendMessageResponse(reply, chunks.Count, remaining));
    }
}
