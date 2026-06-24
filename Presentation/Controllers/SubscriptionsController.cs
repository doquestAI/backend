using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;
using Swashbuckle.AspNetCore.Annotations;
using ActivateRequest = Application.UseCases.Commands.Subscription.ActivateUser.Request;
using ActivateResponse = Application.UseCases.Commands.Subscription.ActivateUser.Response;
using DeactivateRequest = Application.UseCases.Commands.Subscription.DeactivateUser.Request;
using DeactivateResponse = Application.UseCases.Commands.Subscription.DeactivateUser.Response;
using WebhookRequest = Application.UseCases.Commands.Subscription.HandleStripeWebhook.Request;
using WebhookResponse = Application.UseCases.Commands.Subscription.HandleStripeWebhook.Response;

namespace Presentation.Controllers;

[ApiController]
[Route("api/subscriptions")]
internal class SubscriptionsController(IMediator mediator) : InternalControllerBase
{
    [HttpPost("webhooks/stripe")]
    [AllowAnonymous]
    [SwaggerOperation(OperationId = "HandleStripeWebhook")]
    public async Task<ActionResult<WebhookResponse>> HandleStripeWebhook(
        CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var rawPayload = await reader.ReadToEndAsync(cancellationToken);
        var signature = Request.Headers["Stripe-Signature"].ToString();

        var response = await mediator.Send(new WebhookRequest(rawPayload, signature), cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("admin/users/{userId}/activate")]
    [Authorize(Policy = "AdminOnly")]
    [SwaggerOperation(OperationId = "ActivateUser")]
    public async Task<ActionResult<ActivateResponse>> ActivateUser(
        string userId,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ActivateRequest(userId), cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("admin/users/{userId}/deactivate")]
    [Authorize(Policy = "AdminOnly")]
    [SwaggerOperation(OperationId = "DeactivateUser")]
    public async Task<ActionResult<DeactivateResponse>> DeactivateUser(
        string userId,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeactivateRequest(userId), cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}