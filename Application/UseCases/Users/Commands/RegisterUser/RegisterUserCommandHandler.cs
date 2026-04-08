using Application.Common;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.UseCases.Users.Commands.RegisterUser;

internal sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPlanRepository planRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterUserCommand, Result<RegisterUserResponse>>
{
    public async Task<Result<RegisterUserResponse>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        // Idempotent: return existing user if already registered
        var existing = await userRepository.GetByFirebaseUidAsync(request.FirebaseUid, cancellationToken);
        if (existing is not null)
            return Result.Success(new RegisterUserResponse(existing.Id, existing.Email.Address, existing.Plan.Type));

        // Assign Free plan by default
        var freePlan = await planRepository.GetWithParametersAsync(
            p => p.Id == Plan.FreePlanId,
            cancellationToken);

        if (freePlan is null)
            return Result.Failure<RegisterUserResponse>("Plan", "Plano Free não encontrado. Verifique o seed do banco.");

        var user = User.Create(request.FirebaseUid, request.Email, freePlan.Id);
        if (user.IsInvalid)
            return Result.Failure<RegisterUserResponse>(user.Notifications);

        await userRepository.CreateAsync(user, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new RegisterUserResponse(user.Id, user.Email.Address, freePlan.Type));
    }
}