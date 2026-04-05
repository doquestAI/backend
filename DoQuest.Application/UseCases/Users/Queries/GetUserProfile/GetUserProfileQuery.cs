using DoQuest.Application.Common;
using DoQuest.Domain.Enums;
using MediatR;

namespace DoQuest.Application.UseCases.Users.Queries.GetUserProfile;

public sealed record UserProfileDto(
    Guid Id,
    string Email,
    PlanType PlanType,
    string PlanName,
    int DailyMessageCount,
    int DailyMessageLimit,
    DateTime CreatedAt);

public sealed record GetUserProfileQuery(string FirebaseUid)
    : IRequest<Result<UserProfileDto>>;
