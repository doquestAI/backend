using Domain.Agents.Enem;

namespace Domain.Interfaces.Pipelines.Enem;

internal interface IExplainTopicPipeline : IPipeline<ExplainRequest, string>;
