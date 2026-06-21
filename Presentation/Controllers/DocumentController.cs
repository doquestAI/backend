using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;
using Swashbuckle.AspNetCore.Annotations;
using DeleteRequest = Application.UseCases.Commands.Document.Delete.Request;
using DeleteResponse = Application.UseCases.Commands.Document.Delete.Response;
using UploadRequest = Application.UseCases.Commands.Document.Upload.Request;
using UploadResponse = Application.UseCases.Commands.Document.Upload.Response;

namespace Presentation.Controllers;

[ApiController]
[Route("Document")]
[Authorize]
internal class DocumentController(IMediator mediator) : InternalControllerBase
{
    [HttpPost("Upload")]
    [SwaggerOperation(OperationId = "DocumentUploadDocument")]
    [Consumes("multipart/form-data")]
    [RequestFormLimits(
    MultipartBodyLengthLimit = long.MaxValue,
    MultipartHeadersLengthLimit = 1024 * 1024 * 5
    )]
    public async Task<ActionResult<UploadResponse>> Upload(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UploadRequest(file), cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{documentId:guid}")]
    [SwaggerOperation(OperationId = "DocumentDeleteDocument")]
    public async Task<ActionResult<DeleteResponse>> Delete(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteRequest(documentId), cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}