using Domain.Enums;

namespace Application.UseCases.Users.Commands.ChangePlan;

internal sealed record ChangePlanResponse(Guid UserId, PlanType NewPlanType, string NewPlanName);
