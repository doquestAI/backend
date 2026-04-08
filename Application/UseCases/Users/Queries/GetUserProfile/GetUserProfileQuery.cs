using Application.Common;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Users.Queries.GetUserProfile;

internal sealed record UserProfileDto(
    Guid Id,
    string Email,
    PlanType PlanType,
    string PlanName,
    int DailyMessageCount,
    int DailyMessageLimit,
    DateTime CreatedAt);

internal sealed record GetUserProfileQuery(string FirebaseUid)
    : IRequest<Result<UserProfileDto>>;