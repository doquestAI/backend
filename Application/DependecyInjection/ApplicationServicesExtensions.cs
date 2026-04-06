using Domain.Interfaces.Handlers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Application.DependecyInjection;

internal static class ApplicationServicesExtensions
{
    internal static void ConfigureApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(x => x.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        services
            .AddScoped<IEmailNotificationHandler, EmailNotificationHandler>();
        services.AddScoped<IEmbeddingCompletedHandler, EmbeddingCompletedHandler>();
        services.AddScoped<IEmbeddingDeletionCompletedHandler, EmbeddingDeletionCompletedHandler>();
        services.AddScoped<IStorageUploadHandler, StorageUploadHandler>();
    }
}