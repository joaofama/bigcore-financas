using Financas.Application.Queries.Transacoes;
using FluentValidation;

namespace Financas.Application.Validations.Transacoes;

public class ObterTransacoesPorMesQueryValidator : AbstractValidator<ObterTransacoesPorMesQuery>
{
    public ObterTransacoesPorMesQueryValidator()
    {
        RuleFor(q => q.UsuarioId)
            .NotEmpty()
            .WithMessage("O identificador do usuário é obrigatório.");

        RuleFor(q => q.Mes)
            .InclusiveBetween(1, 12)
            .WithMessage("O mês informado é inválido. Deve estar entre 1 e 12.");

        RuleFor(q => q.Ano)
                    .GreaterThan(1753) 
                    .LessThan(9999)  
                    .WithMessage("O ano informado é inválido.");
    }
}