using DoQuest.Application.Common;
using DoQuest.Application.UseCases.Users.Commands.ChangePlan;
using DoQuest.Application.UseCases.Users.Commands.RegisterUser;
using DoQuest.Application.UseCases.Users.Queries.GetUserProfile;
using Flunt.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoQuest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController(IMediator mediator, ICurrentUser currentUser) : ControllerBase
{
    public sealed record RegisterUserRequest(string FirebaseUid, string Email);
    public sealed record ChangePlanRequest(Guid NewPlanId);

    /// <summary>Registers a new user (idempotent — safe to call on every login).</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IReadOnlyList<Notification>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new RegisterUserCommand(request.FirebaseUid, request.Email), ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(result.Notifications);
    }

    /// <summary>Returns the authenticated user's profile and plan details.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IReadOnlyList<Notification>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserProfileQuery(currentUser.FirebaseUid), ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(result.Notifications);
    }

    /// <summary>Changes the authenticated user's plan.</summary>
    [HttpPatch("me/plan")]
    [Authorize]
    [ProducesResponseType(typeof(ChangePlanResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IReadOnlyList<Notification>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePlan([FromBody] ChangePlanRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new ChangePlanCommand(currentUser.FirebaseUid, request.NewPlanId), ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(result.Notifications);
    }
}
