using DoQuest.Application.Common;
using DoQuest.Domain.Enums;
using MediatR;

namespace DoQuest.Application.UseCases.Vestibulares.Queries.ListVestibulares;

public sealed record VestibularDto(Guid Id, string Name, VestibularType Type, int Year, string Description);

public sealed record ListVestibularesQuery : IRequest<Result<IReadOnlyList<VestibularDto>>>;
