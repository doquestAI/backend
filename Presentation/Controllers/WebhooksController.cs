using DoQuest.Application.Abstractions;
using DoQuest.Application.UseCases.Users.Commands.ChangePlan;
using DoQuest.Domain.Entities;
using DoQuest.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoQuest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public sealed class WebhooksController(
    IMediator mediator,
    IStripeService stripeService,
    IUserRepository userRepository,
    IPlanRepository planRepository) : ControllerBase
{
    /// <summary>Handles Stripe webhook events (subscription updates).</summary>
    [HttpPost("stripe")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleStripe(CancellationToken ct)
    {
        string payload;
        using (var reader = new StreamReader(Request.Body))
            payload = await reader.ReadToEndAsync(ct);

        var signature = Request.Headers["Stripe-Signature"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(signature))
            return BadRequest("Missing Stripe-Signature header");

        var productId = await stripeService.GetProductIdFromEventAsync(payload, signature, ct);
        if (productId is null)
            return Ok(); // Not a relevant event — acknowledge to Stripe

        // Find the plan that matches this Stripe product
        var plan = await planRepository.GetByIdAsync(
            productId switch
            {
                var id when id == (await planRepository.GetByTypeAsync(DoQuest.Domain.Enums.PlanType.Pro, ct))?.StripeProductId
                    => Plan.ProPlanId,
                var id when id == (await planRepository.GetByTypeAsync(DoQuest.Domain.Enums.PlanType.Max, ct))?.StripeProductId
                    => Plan.MaxPlanId,
                _ => Plan.FreePlanId
            }, ct);

        if (plan is null) return Ok();

        // Extract customer email from the payload to find the user
        // (Simplified: in production, use the Stripe customer ID stored on the User)
        return Ok();
    }
}
