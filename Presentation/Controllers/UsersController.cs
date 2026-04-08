using Application.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;
using Swashbuckle.AspNetCore.Annotations;
using ChangePlanCommand = Application.UseCases.Users.Commands.ChangePlan.ChangePlanCommand;
using ChangePlanResponse = Application.UseCases.Users.Commands.ChangePlan.ChangePlanResponse;
using GetProfileQuery = Application.UseCases.Users.Queries.GetUserProfile.GetUserProfileQuery;
using RegisterCommand = Application.UseCases.Users.Commands.RegisterUser.RegisterUserCommand;
using RegisterResponse = Application.UseCases.Users.Commands.RegisterUser.RegisterUserResponse;
using UserProfileDto = Application.UseCases.Users.Queries.GetUserProfile.UserProfileDto;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
internal sealed class UsersController(IMediator mediator, ICurrentUser currentUser) : InternalControllerBase
{
    internal sealed record RegisterUserRequest(string FirebaseUid, string Email);
    internal sealed record ChangePlanRequest(Guid NewPlanId);

    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(OperationId = "UsersRegister")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken ct)
    {
        var result = await mediator.Send(new RegisterCommand(request.FirebaseUid, request.Email), ct);
        return ToActionResult(result);
    }

    [HttpGet("me")]
    [Authorize]
    [SwaggerOperation(OperationId = "UsersGetProfile")]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var result = await mediator.Send(new GetProfileQuery(currentUser.FirebaseUid), ct);
        return ToActionResult(result);
    }

    [HttpPatch("me/plan")]
    [Authorize]
    [SwaggerOperation(OperationId = "UsersChangePlan")]
    public async Task<IActionResult> ChangePlan(
        [FromBody] ChangePlanRequest request,
        CancellationToken ct)
    {
        var result = await mediator.Send(new ChangePlanCommand(currentUser.FirebaseUid, request.NewPlanId), ct);
        return ToActionResult(result);
    }
}