namespace Presentation.Common.Api;

internal static class AppExtensions
{
    internal static void ConfigureDevEnvironment(this WebApplication app)
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "BFFKdesk");
            options.RoutePrefix = string.Empty;
        });
    }
}