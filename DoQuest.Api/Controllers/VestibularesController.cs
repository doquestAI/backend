using DoQuest.Application.UseCases.Vestibulares.Queries.ListVestibulares;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoQuest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class VestibularesController(IMediator mediator) : ControllerBase
{
    /// <summary>Lists all available vestibulares.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<VestibularDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var result = await mediator.Send(new ListVestibularesQuery(), ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(result.Notifications);
    }
}
