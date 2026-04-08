using Domain.Enums;

namespace Application.UseCases.Users.Commands.RegisterUser;

internal sealed record RegisterUserResponse(Guid UserId, string Email, PlanType PlanType);