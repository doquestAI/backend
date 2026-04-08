using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;
using Swashbuckle.AspNetCore.Annotations;
using CreateSessionCommand = Application.UseCases.Chat.Commands.CreateSession.CreateChatSessionCommand;
using SendCommand = Application.UseCases.Chat.Commands.SendMessage.SendMessageCommand;
using GetSessionsQuery = Application.UseCases.Chat.Queries.GetSessions.GetChatSessionsQuery;
using GetSessionQuery = Application.UseCases.Chat.Queries.GetSession.GetChatSessionQuery;

namespace Presentation.Controllers;

[ApiController]
[Route("api/chat/sessions")]
[Authorize]
internal sealed class ChatController(IMediator mediator, ICurrentUser currentUser) : InternalControllerBase
{
    internal sealed record CreateSessionRequest(Guid? VestibularId, string? InitialMessage);
    internal sealed record SendMessageRequest(string Message, Guid? VestibularId);

    /// <summary>Create a new chat session.</summary>
    [HttpPost]
    [SwaggerOperation(OperationId = "CreateChatSession")]
    public async Task<IActionResult> CreateSession(
        [FromBody] CreateSessionRequest request,
        CancellationToken ct)
    {
        var command = new CreateSessionCommand(
            currentUser.FirebaseUid,
            request.VestibularId,
            request.InitialMessage);

        var result = await mediator.Send(command, ct);
        return ToActionResult(result);
    }

    /// <summary>List the current user's chat sessions (newest first).</summary>
    [HttpGet]
    [SwaggerOperation(OperationId = "GetChatSessions")]
    public async Task<IActionResult> GetSessions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = new GetSessionsQuery(currentUser.FirebaseUid, page, pageSize);
        var result = await mediator.Send(query, ct);
        return ToActionResult(result);
    }

    /// <summary>Get a single session with its messages.</summary>
    [HttpGet("{sessionId:guid}")]
    [SwaggerOperation(OperationId = "GetChatSession")]
    public async Task<IActionResult> GetSession(
        Guid sessionId,
        [FromQuery] int messageLimit = 50,
        CancellationToken ct = default)
    {
        var query = new GetSessionQuery(currentUser.FirebaseUid, sessionId, messageLimit);
        var result = await mediator.Send(query, ct);
        return ToActionResult(result);
    }

    /// <summary>Send a message and get an AI reply within an existing session.</summary>
    [HttpPost("{sessionId:guid}/messages")]
    [SwaggerOperation(OperationId = "SendChatMessage")]
    public async Task<IActionResult> SendMessage(
        Guid sessionId,
        [FromBody] SendMessageRequest request,
        CancellationToken ct)
    {
        var command = new SendCommand(
            currentUser.FirebaseUid,
            sessionId,
            request.Message,
            request.VestibularId);

        var result = await mediator.Send(command, ct);
        return ToActionResult(result);
    }
}
