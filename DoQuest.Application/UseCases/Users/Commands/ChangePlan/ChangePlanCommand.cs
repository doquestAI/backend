using DoQuest.Application.Common;
using MediatR;

namespace DoQuest.Application.UseCases.Users.Commands.ChangePlan;

public sealed record ChangePlanCommand(string FirebaseUid, Guid NewPlanId)
    : IRequest<Result<ChangePlanResponse>>;
