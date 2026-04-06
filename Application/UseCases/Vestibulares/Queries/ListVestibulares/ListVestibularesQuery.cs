using Application.Common;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Vestibulares.Queries.ListVestibulares;

internal sealed record VestibularDto(Guid Id, string Name, VestibularType Type, int Year, string Description);

internal sealed record ListVestibularesQuery : IRequest<Result<IReadOnlyList<VestibularDto>>>;
