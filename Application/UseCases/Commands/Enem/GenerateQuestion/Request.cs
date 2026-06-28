using MediatR;

namespace Application.UseCases.Commands.Enem.GenerateQuestion;

internal record Request(string Topic, string Area, string Difficulty = "Médio")
    : IRequest<Response>;
