using Domain.Agents.Enem;

namespace Domain.Interfaces.Pipelines.Enem;

public interface IExplainTopicPipeline : IPipeline<ExplainRequest, string>;
