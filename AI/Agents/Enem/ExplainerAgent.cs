using AI.Agents.Base;
using AI.Common.Config;
using AI.Providers.Session;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AI.Agents.Enem;

/// <summary>Explica teoria de tópicos do ENEM em detalhes.</summary>
public sealed class ExplainerAgent(
    IChatClient client,
    [FromKeyedServices(AgentKeys.Explainer)] AgentConfig config,
    AgentSessionCache sessionCache,
    ILoggerFactory loggerFactory)
    : AgentBase(client, config, sessionCache, loggerFactory);
