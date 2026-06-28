using Domain.Agents.Enem;

namespace Domain.Interfaces.Pipelines.Enem;

internal interface IGenerateQuestionPipeline : IPipeline<QuestionRequest, EnemQuestion>;
