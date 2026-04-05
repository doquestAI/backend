using DoQuest.Application.Common;
using DoQuest.Application.UseCases.Chat.Commands.SendMessage;
using Flunt.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

namespace DoQuest.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ChatController(IMediator mediator, ICurrentUser currentUser) : ControllerBase
{
    public sealed record SendMessageRequest(
        string Message,
        Guid? VestibularId,
        IEnumerable<ChatMessage>? History);

    /// <summary>Sends a message to the educational chatbot with RAG.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(SendMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IReadOnlyList<Notification>), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Send([FromBody] SendMessageRequest request, CancellationToken ct)
    {
        var command = new SendMessageCommand(
            currentUser.FirebaseUid,
            request.Message,
            request.VestibularId,
            request.History ?? []);

        var result = await mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(result.Notifications);
    }
}
