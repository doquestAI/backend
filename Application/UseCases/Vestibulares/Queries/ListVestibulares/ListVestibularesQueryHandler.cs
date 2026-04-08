using Application.Common;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Vestibulares.Queries.ListVestibulares;

internal sealed class ListVestibularesQueryHandler(IVestibularRepository vestibularRepository)
    : IRequestHandler<ListVestibularesQuery, Result<IReadOnlyList<VestibularDto>>>
{
    public async Task<Result<IReadOnlyList<VestibularDto>>> Handle(
        ListVestibularesQuery request,
        CancellationToken cancellationToken)
    {
        var vestibulares = await vestibularRepository.GetAllAsync(cancellationToken);

        var dtos = vestibulares
            .Select(v => new VestibularDto(v.Id, v.Name.Value, v.Type, v.Year.Value, v.Description.Value))
            .ToList()
            .AsReadOnly();

        return Result.Success<IReadOnlyList<VestibularDto>>(dtos);
    }
}