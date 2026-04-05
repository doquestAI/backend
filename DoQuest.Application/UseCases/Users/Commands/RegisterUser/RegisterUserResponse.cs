using DoQuest.Domain.Enums;

namespace DoQuest.Application.UseCases.Users.Commands.RegisterUser;

public sealed record RegisterUserResponse(Guid UserId, string Email, PlanType PlanType);
