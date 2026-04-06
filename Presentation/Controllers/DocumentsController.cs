using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;
using Swashbuckle.AspNetCore.Annotations;
using IngestCommand = Application.UseCases.Documents.Commands.IngestDocument.IngestDocumentCommand;
using IngestResponse = Application.UseCases.Documents.Commands.IngestDocument.IngestDocumentResponse;
using GetByVestibularQuery = Application.UseCases.Documents.Queries.GetDocumentsByVestibular.GetDocumentsByVestibularQuery;
using DocumentSummaryDto = Application.UseCases.Documents.Queries.GetDocumentsByVestibular.DocumentSummaryDto;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
internal sealed class DocumentsController(IMediator mediator) : InternalControllerBase
{
    internal sealed record IngestDocumentRequest(
        string Title,
        string SourceUrl,
        Guid VestibularId,
        string RawText);

    [HttpPost]
    [SwaggerOperation(OperationId = "DocumentsIngest")]
    public async Task<IActionResult> Ingest(
        [FromBody] IngestDocumentRequest request,
        CancellationToken ct)
    {
        var command = new IngestCommand(request.Title, request.SourceUrl, request.VestibularId, request.RawText);
        var result = await mediator.Send(command, ct);
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
