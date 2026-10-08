using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Services;
using Todo.Infrastructure.Modules;

namespace Todo.Application.Modules;

public static class ApplicationModules
{
    public static IServiceCollection AddApplicationModules(this IServiceCollection services)
    {
        services.AddInfrastructureModules();
        services.AddScoped<ITodoService, TodoService>();
        return services;
    }
}
