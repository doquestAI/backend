namespace Domain.Interfaces.Services.AI;

internal interface IChatAgentService
{
    Task<ChatAgentResponse> ProcessMessageAsync(
        Guid sessionId,
        string userMessage,
        Guid? vestibularId,
        CancellationToken ct = default);

    IAsyncEnumerable<string> StreamMessageAsync(
        Guid sessionId,
        string userMessage,
        Guid? vestibularId,
        CancellationToken ct = default);
}

internal sealed record ChatAgentResponse(
    string Reply,
    int SourceChunksUsed,
    Guid SessionId);
