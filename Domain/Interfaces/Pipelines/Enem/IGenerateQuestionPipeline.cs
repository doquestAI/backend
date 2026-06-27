using Domain.Agents.Enem;

namespace Domain.Interfaces.Pipelines.Enem;

public interface IGenerateQuestionPipeline : IPipeline<QuestionRequest, EnemQuestion>;
