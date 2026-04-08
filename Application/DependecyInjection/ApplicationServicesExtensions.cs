using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.DependecyInjection;

internal static class ApplicationServicesExtensions
{
    internal static void ConfigureApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
    }
}