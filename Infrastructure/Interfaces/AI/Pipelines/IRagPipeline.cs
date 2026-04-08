using Domain.Records;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System;

namespace Infrastructure.Interfaces.AI.Pipelines;

internal interface IRagPipeline
{
    Task<(string Reply, RagContext Context)> ExecuteAsync(
        string userMessage,
        Guid? vestibularId,
        AgentSession agentSession,
        IChatClient? queryRewriteClient = null,
        CancellationToken ct = default);
}
