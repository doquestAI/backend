using DoQuest.Domain.Enums;

namespace DoQuest.Application.UseCases.Users.Commands.ChangePlan;

public sealed record ChangePlanResponse(Guid UserId, PlanType NewPlanType, string NewPlanName);
