using Application.Common;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Users.Commands.ChangePlan;

internal sealed class ChangePlanCommandHandler(
    IUserRepository userRepository,
    IPlanRepository planRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ChangePlanCommand, Result<ChangePlanResponse>>
{
    public async Task<Result<ChangePlanResponse>> Handle(
        ChangePlanCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByFirebaseUidAsync(request.FirebaseUid, cancellationToken);
        if (user is null)
            return Result.Failure<ChangePlanResponse>("User", "Usuário não encontrado");

        var plan = await planRepository.GetWithParametersAsync(
            p => p.Id == request.NewPlanId,
            cancellationToken);

        if (plan is null)
            return Result.Failure<ChangePlanResponse>("Plan", "Plano não encontrado");

        user.ChangePlan(plan.Id);
        if (user.IsInvalid)
            return Result.Failure<ChangePlanResponse>(user.Notifications);

        userRepository.Update(user);
        await unitOfWork.CommitAsync(cancellationToken);
        return Result.Success(new ChangePlanResponse(user.Id, plan.Type, plan.Name.Value));
    }
}
