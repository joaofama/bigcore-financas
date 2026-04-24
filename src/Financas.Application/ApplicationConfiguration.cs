using Microsoft.Extensions.DependencyInjection;

namespace Financas.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ApplicationConfiguration).Assembly));

        return services;
    }
}