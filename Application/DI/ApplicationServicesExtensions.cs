using Application.UseCases.Commands.EmailNotification;
using Application.UseCases.Commands.Storage;
using Domain.Interfaces.Handlers;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using EmbeddingCompletedHandler = Application.UseCases.Commands.Embeddings.EmbeddingCompletedHandler;
using EmbeddingDeletionCompletedHandler = Application.UseCases.Commands.Embeddings.EmbeddingDeletionCompletedHandler;

namespace Application.DI;

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