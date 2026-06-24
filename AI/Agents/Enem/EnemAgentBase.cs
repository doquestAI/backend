using AI.Agents.Base;
using AI.Providers.Session;
using Microsoft.Agents.AI;

namespace AI.Agents.Enem;

/// <summary>Wrapper especializado para agentes ENEM cuja saída é texto livre.</summary>
public abstract class EnemTextAgent<TIn>(AIAgent inner, AgentSessionCache sessionCache)
    : AgentWrapperBase<TIn, string>(inner, sessionCache)
{
    protected override string ParseResponse(AgentResponse response)
        => ResponseText(response);
}
