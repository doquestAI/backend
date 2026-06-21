using Application.UseCases.Commands.User.Delete;
using Application.UseCases.Commands.User.Update;
using Domain.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;
using Swashbuckle.AspNetCore.Annotations;
using ActivatePasswordRequest = Application.UseCases.Commands.User.ForgotPassword.Activate.Request;
using ActivateRequest = Application.UseCases.Commands.User.Activate.Request;
using ForgotRequest = Application.UseCases.Commands.User.ForgotPassword.Request;
using GetAllRequest = Application.UseCases.Queries.User.GetAll.Request;
using GetAllResponse = Application.UseCases.Queries.User.GetAll.Response;
using GetOneRequest = Application.UseCases.Queries.User.GetOne.Request;
using GetOneResponse = Application.UseCases.Queries.User.GetOne.Response;
using LoginRequest = Application.UseCases.Commands.User.Login.Request;
using LoginResponse = Application.UseCases.Commands.User.Login.Response;
using LogoutRequest = Application.UseCases.Commands.User.Logout.Request;
using RefreshTokenRequest = Application.UseCases.Commands.User.RefreshToken.Request;
using RefreshTokenResponse = Application.UseCases.Commands.User.RefreshToken.Response;
using RegisterRequest = Application.UseCases.Commands.User.Register.Request;
using ResendCodeRequest = Application.UseCases.Commands.User.ResendCode.Request;

namespace Presentation.Controllers;

[ApiController]
[Route("User")]
internal class UserController(IMediator mediator) : InternalControllerBase
{
    [HttpGet]
    [SwaggerOperation(OperationId = "UserGetAll")]
    [Authorize(Roles = "admin-company, admin-super")]
    public async Task<ActionResult<GetAllResponse>> GetAll(
        [FromQuery] GetAllRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(OperationId = "UserGetOne")]
    [Authorize(Roles = "admin-company, admin-super")]
    public async Task<ActionResult<GetOneResponse>> GetOne(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetOneRequest(id), cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("Login")]
    [SwaggerOperation(OperationId = "UserLogin")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("Register")]
    [SwaggerOperation(OperationId = "UserRegister")]
    public async Task<ActionResult<BaseResponse>> Register([FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("Activate")]
    [SwaggerOperation(OperationId = "UserActivate")]
    public async Task<ActionResult<BaseResponse>> Activate([FromBody] ActivateRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("Forgot-Password")]
    [SwaggerOperation(OperationId = "UserForgotPassword")]
    public async Task<ActionResult<BaseResponse>> ForgotPassword([FromQuery] ForgotRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("Resend-Code")]
    [SwaggerOperation(OperationId = "UserResendCode")]
    public async Task<ActionResult<BaseResponse>> ResendCode([FromBody] ResendCodeRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("Forgot-Password/Activate")]
    [SwaggerOperation(OperationId = "UserForgotPasswordActivate")]
    public async Task<ActionResult<BaseResponse>> ForgotPasswordActivate([FromBody] ActivatePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("Logout")]
    [SwaggerOperation(OperationId = "UserLogout")]
    public async Task<ActionResult<BaseResponse>> Logout([FromBody] LogoutRequest request,
         CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("Refresh-Token")]
    [SwaggerOperation(OperationId = "UserRefreshToken")]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("Delete")]
    [SwaggerOperation(OperationId = "UserDelete")]
    [Authorize(Roles = "admin-company, admin-super")]
    public async Task<ActionResult<BaseResponse>> DeleteUser([FromBody] DeleteUserRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("Update")]
    [SwaggerOperation(OperationId = "UserUpdate")]
    public async Task<ActionResult<BaseResponse>> UserUpdate([FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}