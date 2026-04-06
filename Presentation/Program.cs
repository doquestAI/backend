using Infrastructure.Configurations;
using Presentation.Common.Api;
using Presentation.Middleware;


var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, options) =>
{
    var env = context.HostingEnvironment;
    if (env.IsDevelopment())
    {
        options.ListenAnyIP(7069, listenOptions =>
        {
            listenOptions.UseHttps();
        });
    }
    else
    {
        var kestrelSection = context.Configuration.GetSection("Kestrel:Endpoints:Https:Certificate");
        var certPath = kestrelSection.GetValue<string>("Path");
        var certPassword = kestrelSection.GetValue<string>("Password");
        options.ListenAnyIP(7069, listenOptions =>
        {
            if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(certPassword))
            {
                listenOptions.UseHttps(certPath, certPassword);
            }
            else
            {
                listenOptions.UseHttps();
            }
        });
    }
    options.ListenAnyIP(5070);
});

builder.AddConfiguration();
builder.AddRateLimiting();
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>()
    ?? throw new InvalidOperationException("AppSettings not configured");

builder.AddCrossOrigin(appSettings);
builder.AddAuthentication();
builder.Services.AddDataContexts();
builder.AddServices();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.ConfigureDevEnvironment();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseForwardedHeaders();

app.UseRouting();
app.UseMiddleware<ExceptionHandler>();
app.UseCors(appSettings.CorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
