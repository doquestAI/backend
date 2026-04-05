using System.Net;
using System.Text.Json;

namespace DoQuest.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new { error = "Ocorreu um erro interno. Tente novamente mais tarde." };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
