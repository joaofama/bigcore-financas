using FluentValidation;
using Financas.Application.Queries.Dashboard;

namespace Financas.Application.Validators.Dashboard;

public class ObterResumoDashboardQueryValidator : AbstractValidator<ObterResumoDashboardQuery>
{
    public ObterResumoDashboardQueryValidator()
    {
        RuleFor(x => x.Mes)
            .InclusiveBetween(1, 12)
            .WithMessage("O mês deve estar entre 1 e 12.");

        RuleFor(x => x.Ano)
            .GreaterThan(2000)
            .WithMessage("O ano informado é inválido.");

        RuleFor(x => x.UsuarioId)
            .NotEmpty()
            .WithMessage("O ID do usuário é obrigatório para consultar o dashboard.");
    }
}