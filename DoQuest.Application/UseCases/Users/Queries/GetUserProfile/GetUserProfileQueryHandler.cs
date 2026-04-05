using DoQuest.Application.Common;
using DoQuest.Domain.Repositories;
using MediatR;

namespace DoQuest.Application.UseCases.Users.Queries.GetUserProfile;

public sealed class GetUserProfileQueryHandler(IUserRepository userRepository)
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
            user.Email,
            user.Plan.Type,
            user.Plan.Name,
            user.DailyMessageCount,
            user.Plan.DailyMessageLimit,
            user.CreatedAt));
    }
}
