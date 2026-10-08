using Microsoft.Extensions.DependencyInjection;
using Todo.Domain.Repositories;
using Todo.Infrastructure.Repositories;

namespace Todo.Infrastructure.Modules;

public static class InfrastructureModules
{
    public static IServiceCollection AddInfrastructureModules(this IServiceCollection services)
    {
        services.AddScoped<ITodoRepository, TodoRepository>();
        return services;
    }
}
