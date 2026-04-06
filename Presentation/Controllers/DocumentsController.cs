using DoQuest.Application.Common;
using DoQuest.Application.UseCases.Documents.Commands.IngestDocument;
using DoQuest.Application.UseCases.Documents.Queries.GetDocumentsByVestibular;
using Flunt.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoQuest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class DocumentsController(IMediator mediator) : ControllerBase
{
    public sealed record IngestDocumentRequest(
        string Title,
        string SourceUrl,
        Guid VestibularId,
        string RawText);

    /// <summary>Ingests a study document: chunks the text and generates embeddings for RAG.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(IngestDocumentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(IReadOnlyList<Notification>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Ingest([FromBody] IngestDocumentRequest request, CancellationToken ct)
    {
        var command = new IngestDocumentCommand(request.Title, request.SourceUrl, request.VestibularId, request.RawText);
        var result = await mediator.Send(command, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetByVestibular), new { vestibularId = request.VestibularId }, result.Value)
            : UnprocessableEntity(result.Notifications);
    }

    /// <summary>Lists all documents for a given vestibular.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DocumentSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByVestibular([FromQuery] Guid vestibularId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetDocumentsByVestibularQuery(vestibularId), ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(result.Notifications);
    }
}
