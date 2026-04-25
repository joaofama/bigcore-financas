using Financas.Application.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Financas.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 1. Registra automaticamente todos os validadores (classes que herdam de AbstractValidator)
        services.AddValidatorsFromAssembly(typeof(ApplicationConfiguration).Assembly);

        // 2. Configura o MediatR e injeta o pipeline de validação
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ApplicationConfiguration).Assembly);

            // É esta linha que "liga" a sua classe ValidationBehavior no fluxo!
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        return services;
    }
}