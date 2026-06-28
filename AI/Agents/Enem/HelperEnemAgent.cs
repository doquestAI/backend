using AI.Agents.Base;
using AI.Common.Config;
using AI.Providers.Session;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AI.Agents.Enem;

/// <summary>Responde dúvidas gerais sobre o ENEM. Texto livre, suporta streaming.</summary>
public sealed class HelperEnemAgent(
    IChatClient client,
    [FromKeyedServices(AgentKeys.Helper)] AgentConfig config,
    AgentSessionCache sessionCache,
    ILoggerFactory loggerFactory)
    : AgentBase(client, config, sessionCache, loggerFactory);
