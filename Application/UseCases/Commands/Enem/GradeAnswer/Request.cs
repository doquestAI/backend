using MediatR;

namespace Application.UseCases.Commands.Enem.GradeAnswer;

internal record Request(
    string Question,
    string StudentAnswer,
    string CorrectAnswer,
    string Area = "Geral") : IRequest<Response>;
