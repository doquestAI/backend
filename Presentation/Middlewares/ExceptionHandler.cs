namespace Presentation.Middlewares;

internal class ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger, IHostEnvironment env)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandler> _logger = logger;
    private readonly IHostEnvironment _env = env;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro inesperado");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new
            {
                Message = "Ocorreu um erro interno no servidor. Tente novamente mais tarde.",
                devMessage = _env.IsDevelopment() ? new
                {
                    exception = ex.GetType().Name,
                    Message = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                } : null
            };
            await context.Response.WriteAsJsonAsync(response.ToString());
        }
    }
}