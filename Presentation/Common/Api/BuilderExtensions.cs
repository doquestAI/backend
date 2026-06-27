using AI.Agents.Extensions;
using Application.DI;
using Domain.Configurations;
using Domain.Interfaces.Context;
using Infrastructure.DI;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Identity.Web;
using Microsoft.OpenApi;
using Newtonsoft.Json.Serialization;
using Presentation.Adapters;
using Presentation.Common.Converters;
using Presentation.Common.Filters;
using Presentation.Filters;
using System.Text.Json.Serialization;

namespace Presentation.Common.Api;

internal static class BuilderExtensions
{
    internal static void AddConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
        builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
        builder.Services.Configure<AzureSettings>(builder.Configuration.GetSection("AzureStorageSettings"));
        builder.Services.Configure<AzureAdSettings>(builder.Configuration.GetSection("AzureAd"));
        builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
        builder.Services.Configure<ServiceBusSettings>(builder.Configuration.GetSection("ServiceBusSettings"));
        builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
        builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection("CacheSettings"));
        builder.Services.Configure<SignedUrlSettings>(builder.Configuration.GetSection("SignedUrlSettings"));
        builder.Services.Configure<RateLimitSettings>(builder.Configuration.GetSection("RateLimitSettings"));
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add(new ProducesAttribute("application/json"));
            options.ReturnHttpNotAcceptable = true;
        })
        .ConfigureApplicationPartManager(manager =>
        {
            manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .Select(e => new
                    {
                        field = e.Key,
                        errors = e.Value.Errors.Select(err => err.ErrorMessage)
                    });

                var result = new
                {
                    Message = "Invalid Request.",
                    Notifications = errors
                };
                return new BadRequestObjectResult(result);
            };
        })
        .AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        })
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            options.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore;
            options.SerializerSettings.ContractResolver = new IgnoreEmptyEnumerablesContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
    }

    internal static void AddEntraAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddMicrosoftIdentityWebApiAuthentication(
            builder.Configuration,
            jwtBearerScheme: "Bearer",
            configSectionName: "AzureAd");

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("admin"));
        });
    }

    internal static void AddRateLimiting(this WebApplicationBuilder builder)
    {
        var rateLimitSettings = builder.Configuration.GetSection("RateLimitSettings").Get<RateLimitSettings>();
        if (rateLimitSettings?.EnableRateLimit != true)
            return;

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            foreach (var policy in rateLimitSettings.Policies)
            {
                options.AddFixedWindowLimiter(policy.Key, limiterOptions =>
                {
                    limiterOptions.PermitLimit = policy.Value.PermitLimit;
                    limiterOptions.Window = TimeSpan.FromSeconds(policy.Value.WindowSizeSeconds);
                    limiterOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    limiterOptions.QueueLimit = 2;
                });
            }

            options.OnRejected = async (context, _) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    statusCode = 429,
                    message = "Muitas requisições. Tente novamente em alguns segundos."
                });
            };
        });
    }

    internal static void AddCrossOrigin(this WebApplicationBuilder builder, AppSettings configuration)
    {
        builder.Services.AddCors(
            options => options.AddPolicy(
                configuration.CorsPolicyName,
                policy => policy
                    .WithOrigins([
                        configuration.BackendUrl,
                        configuration.FrontendUrl,
                        configuration.FrontendUrl.Replace("http://", "https://"),
                    ])
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
            ));
    }

    internal static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient();
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "BFFKdesk",
                    Version = "v1"
                });

                options.DocumentFilter<ExternalSwaggerDocumentFilter>();
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme"
                });

                options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", null),
                        new List<string>()
                    }
                });

                options.OperationFilter<BaseResponseOperationFilter>();
                options.EnableAnnotations();
                options.CustomSchemaIds(type =>
                {
                    if (type.IsGenericType)
                    {
                        var genericTypeName = type.Name.Substring(0, type.Name.IndexOf('`'));
                        var genericArgs = string.Join("", type.GenericTypeArguments.Select(GetSimplifiedName));
                        return $"{genericTypeName}{genericArgs}";
                    }
                    return GetSimplifiedName(type);

                    static string GetSimplifiedName(Type t)
                    {
                        if (t.FullName != null && t.FullName.Contains("UseCases"))
                        {
                            var parts = t.FullName.Split('.');
                            var useCasesIndex = Array.IndexOf(parts, "UseCases");

                            if (useCasesIndex >= 0 && useCasesIndex + 3 < parts.Length)
                            {
                                var entity = parts[useCasesIndex + 2];
                                var action = parts[useCasesIndex + 3];
                                return $"{entity}{action}{t.Name}";
                            }
                        }
                        return t.Name;
                    }
                });
            });
        }

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownProxies.Clear();
        });

        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 1024L * 1024 * 500;
            options.MultipartHeadersLengthLimit = 1024 * 1024 * 5;
            options.MultipartBoundaryLengthLimit = 1024;
        });

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
            options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(1);
        });

        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = 1024 * 1024 * 500;
            options.Limits.MaxRequestHeadersTotalSize = 1024 * 1024;
        });
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });

        builder.Services.ConfigureApplicationServices();
        builder.Services.ConfigureInfrastructureServices();
        builder.Services.AddAllBackgroundServices();

        // ── Microsoft Agent Framework — wiring para o pipeline ENEM ─────────
        // Plugue aqui o provider escolhido (OpenAI / Azure OpenAI / Ollama / Foundry)
        // e o vector store (Qdrant / Azure AI Search / Pinecone).
        // Enquanto não plugado, as rotas /Enem/* retornam 500 com mensagem clara.
        builder.Services.AddAgents(opt =>
        {
            // opt.ChatClientBuilder = new OpenAIChatClient(...);
            // opt.EmbeddingGeneratorBuilder = new OpenAIEmbeddingGenerator(...);
            // opt.VectorStoreFactory = sp => new QdrantVectorStore(...);
        });

        var cacheSettings = builder.Configuration.GetSection("CacheSettings").Get<CacheSettings>() ?? new CacheSettings();
        builder.Services.AddCacheServices(cacheSettings);

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IUserContext, HttpUserContext>();
    }
}