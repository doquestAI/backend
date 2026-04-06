using Application.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Presentation.Common;
using Swashbuckle.AspNetCore.Annotations;
using SendCommand = Application.UseCases.Chat.Commands.SendMessage.SendMessageCommand;
using SendResponse = Application.UseCases.Chat.Commands.SendMessage.SendMessageResponse;

namespace Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
internal sealed class ChatController(IMediator mediator, ICurrentUser currentUser) : InternalControllerBase
{
    internal sealed record SendMessageRequest(
        string Message,
        Guid? VestibularId,
        IEnumerable<ChatMessage>? History);

    [HttpPost]
    [SwaggerOperation(OperationId = "ChatSend")]
    public async Task<IActionResult> Send(
        [FromBody] SendMessageRequest request,
        CancellationToken ct)
    {
        var command = new SendCommand(
            currentUser.FirebaseUid,
            request.Message,
            request.VestibularId,
            request.History ?? []);

        var result = await mediator.Send(command, ct);
        return ToActionResult(result);
    }
}
