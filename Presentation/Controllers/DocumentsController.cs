using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;
using Swashbuckle.AspNetCore.Annotations;
using DocumentSummaryDto = Application.UseCases.Documents.Queries.GetDocumentsByVestibular.DocumentSummaryDto;
using GetByVestibularQuery = Application.UseCases.Documents.Queries.GetDocumentsByVestibular.GetDocumentsByVestibularQuery;
using IngestCommand = Application.UseCases.Documents.Commands.IngestDocument.IngestDocumentCommand;
using IngestResponse = Application.UseCases.Documents.Commands.IngestDocument.IngestDocumentResponse;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
internal sealed class DocumentsController(IMediator mediator) : InternalControllerBase
{

    [HttpPost]
    [SwaggerOperation(OperationId = "DocumentUpload")]
    public async Task<IActionResult> Ingest(
        [FromBody] request,
        CancellationToken ct)
    {
        return ToActionResult(result, StatusCodes.Status201Created);
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "DocumentsGetByVestibular")]
    public async Task<IActionResult> GetByVestibular(
        [FromQuery] Guid vestibularId,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetByVestibularQuery(vestibularId), ct);
        return ToActionResult(result);
    }
}