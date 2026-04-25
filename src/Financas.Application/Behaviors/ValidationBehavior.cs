using FluentValidation;
using MediatR;

namespace Financas.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            // Executa todos os validadores encontrados para este request simultaneamente
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            // Extrai todos os erros (se houver) de todos os resultados
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            // Se a lista de erros não estiver vazia, interrompe o fluxo e lança a exceção
            if (failures.Count != 0)
            {
                // Usando a referência explícita para evitar conflito com DataAnnotations
                throw new FluentValidation.ValidationException(failures);
            }
        }

        // Se passou em todas as validações, continua o fluxo normal chamando o Handler
        return await next();
    }
}