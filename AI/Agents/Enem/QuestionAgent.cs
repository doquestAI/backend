using AI.Agents.Base;
using AI.Common.Config;
using AI.Providers.Session;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AI.Agents.Enem;

/// <summary>Gera questões estilo ENEM com gabarito.</summary>
public sealed class QuestionAgent(
    IChatClient client,
    [FromKeyedServices(AgentKeys.QuestionGenerator)] AgentConfig config,
    AgentSessionCache sessionCache,
    ILoggerFactory loggerFactory)
    : AgentBase(client, config, sessionCache, loggerFactory);
