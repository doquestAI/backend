using DoQuest.Application.Common;
using DoQuest.Domain.Repositories;
using MediatR;

namespace DoQuest.Application.UseCases.Vestibulares.Queries.ListVestibulares;

public sealed class ListVestibularesQueryHandler(IVestibularRepository vestibularRepository)
    : IRequestHandler<ListVestibularesQuery, Result<IReadOnlyList<VestibularDto>>>
{
    public async Task<Result<IReadOnlyList<VestibularDto>>> Handle(
        ListVestibularesQuery request,
        CancellationToken cancellationToken)
    {
        var vestibulares = await vestibularRepository.GetAllAsync(cancellationToken);

        var dtos = vestibulares
            .Select(v => new VestibularDto(v.Id, v.Name, v.Type, v.Year, v.Description))
            .ToList()
            .AsReadOnly();

        return Result.Success<IReadOnlyList<VestibularDto>>(dtos);
    }
}
