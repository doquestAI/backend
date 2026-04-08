using Application.Common;
using MediatR;

namespace Application.UseCases.Users.Commands.ChangePlan;

internal sealed record ChangePlanCommand(string FirebaseUid, Guid NewPlanId)
    : IRequest<Result<ChangePlanResponse>>;