using Application.Pipelines.Abstractions;
using Domain.Agents.Enem;

namespace Application.Pipelines.Enem.Abstractions;

public interface IGenerateQuestionPipeline : IPipeline<QuestionRequest, EnemQuestion>;
