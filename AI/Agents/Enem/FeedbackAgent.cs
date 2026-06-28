using AI.Agents.Base;
using AI.Common.Config;
using AI.Providers.Session;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AI.Agents.Enem;

/// <summary>Corrige e avalia respostas do candidato.</summary>
internal sealed class FeedbackAgent(
    IChatClient client,
    [FromKeyedServices(AgentKeys.Feedback)] AgentConfig config,
    AgentSessionCache sessionCache,
    ILoggerFactory loggerFactory)
    : AgentBase(client, config, sessionCache, loggerFactory);
