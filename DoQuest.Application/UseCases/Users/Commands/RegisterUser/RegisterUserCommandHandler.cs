using DoQuest.Application.Common;
using DoQuest.Domain.Entities;
using DoQuest.Domain.Repositories;
using MediatR;

namespace DoQuest.Application.UseCases.Users.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler(
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
            return Result.Success(new RegisterUserResponse(existing.Id, existing.Email, existing.Plan.Type));

        // Assign Free plan by default
        var freePlan = await planRepository.GetByIdAsync(Plan.FreePlanId, cancellationToken);
        if (freePlan is null)
            return Result.Failure<RegisterUserResponse>("Plan", "Plano Free não encontrado. Verifique o seed do banco.");

        var user = User.Create(request.FirebaseUid, request.Email, freePlan.Id);
        if (user.IsInvalid)
            return Result.Failure<RegisterUserResponse>(user.Notifications);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new RegisterUserResponse(user.Id, user.Email, freePlan.Type));
    }
}
