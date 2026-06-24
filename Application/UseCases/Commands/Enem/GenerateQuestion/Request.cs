using MediatR;

namespace Application.UseCases.Commands.Enem.GenerateQuestion;

public record Request(string Topic, string Area, string Difficulty = "Médio")
    : IRequest<Response>;
