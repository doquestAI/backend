using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
internal sealed class WebhooksController(
    IMediator mediator,
    IStripeService stripeService,
    IUserRepository userRepository,
    IPlanRepository planRepository) : InternalControllerBase
{
    [HttpPost("stripe")]
    [SwaggerOperation(OperationId = "WebhooksHandleStripe")]
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
            return Ok();

        var plan = await planRepository.GetByIdAsync(
            productId switch
            {
                var id when id == (await planRepository.GetByTypeAsync(Domain.Enums.PlanType.Pro, ct))?.StripeProductId.Value
                    => Plan.ProPlanId,
                var id when id == (await planRepository.GetByTypeAsync(Domain.Enums.PlanType.Max, ct))?.StripeProductId.Value
                    => Plan.MaxPlanId,
                _ => Plan.FreePlanId
            }, ct);

        if (plan is null)
            return Ok();

        return Ok();
    }
}
