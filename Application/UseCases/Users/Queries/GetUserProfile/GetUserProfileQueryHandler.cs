using Application.Common;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Users.Queries.GetUserProfile;

internal sealed class GetUserProfileQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    public async Task<Result<UserProfileDto>> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByFirebaseUidAsync(request.FirebaseUid, cancellationToken);
        if (user is null)
            return Result.Failure<UserProfileDto>("User", "Usuário não encontrado");

        return Result.Success(new UserProfileDto(
            user.Id,
            user.Email.Address,
            user.Plan.Type,
            user.Plan.Name.Value,
            user.DailyMessageCount,
            user.Plan.DailyMessageLimit.Value,
            user.CreatedAt));
    }
}