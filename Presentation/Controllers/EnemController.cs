using Domain.Interfaces.Pipelines.Enem;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common;
using Swashbuckle.AspNetCore.Annotations;
using AskHelperRequest = Application.UseCases.Commands.Enem.AskHelper.Request;
using AskHelperResponse = Application.UseCases.Commands.Enem.AskHelper.Response;
using ExplainTopicRequest = Application.UseCases.Commands.Enem.ExplainTopic.Request;
using ExplainTopicResponse = Application.UseCases.Commands.Enem.ExplainTopic.Response;
using GenerateQuestionRequest = Application.UseCases.Commands.Enem.GenerateQuestion.Request;
using GenerateQuestionResponse = Application.UseCases.Commands.Enem.GenerateQuestion.Response;
using GradeAnswerRequest = Application.UseCases.Commands.Enem.GradeAnswer.Request;
using GradeAnswerResponse = Application.UseCases.Commands.Enem.GradeAnswer.Response;

namespace Presentation.Controllers;

[ApiController]
[Route("Enem")]
[Authorize]
internal class EnemController(
    IMediator mediator,
    IAskHelperPipeline askHelperPipeline) : InternalControllerBase
{
    [HttpPost("Question/Generate")]
    [SwaggerOperation(OperationId = "EnemGenerateQuestion")]
    public async Task<ActionResult<GenerateQuestionResponse>> GenerateQuestion(
        GenerateQuestionRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("Question/Grade")]
    [SwaggerOperation(OperationId = "EnemGradeAnswer")]
    public async Task<ActionResult<GradeAnswerResponse>> GradeAnswer(
        GradeAnswerRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("Topic/Explain")]
    [SwaggerOperation(OperationId = "EnemExplainTopic")]
    public async Task<ActionResult<ExplainTopicResponse>> ExplainTopic(
        ExplainTopicRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("Helper/Ask")]
    [SwaggerOperation(OperationId = "EnemAskHelper")]
    public async Task<ActionResult<AskHelperResponse>> AskHelper(
        AskHelperRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("Helper/Ask/Stream")]
    [SwaggerOperation(OperationId = "EnemAskHelperStream")]
    [Produces("text/event-stream")]
    public async Task AskHelperStream(
        [FromBody] AskHelperRequest request,
        CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers["X-Accel-Buffering"] = "no";

        await foreach (var chunk in askHelperPipeline.RunStreamingAsync(
            request.Question, cancellationToken))
        {
            await Response.WriteAsync($"data: {chunk}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }

        await Response.WriteAsync("event: done\ndata: [DONE]\n\n", cancellationToken);
    }
}
