using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;
using Swashbuckle.AspNetCore.Annotations;
using ListQuery = Application.UseCases.Vestibulares.Queries.ListVestibulares.ListVestibularesQuery;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
internal sealed class VestibularesController(IMediator mediator) : InternalControllerBase
{
    [HttpGet]
    [SwaggerOperation(OperationId = "VestibularesList")]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var result = await mediator.Send(new ListQuery(), ct);
        return ToActionResult(result);
    }
}