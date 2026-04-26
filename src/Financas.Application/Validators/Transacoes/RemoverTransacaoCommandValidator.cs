using FluentValidation;
using Financas.Application.Commands.Transacoes;

namespace Financas.Application.Validators.Transacoes;

public class RemoverTransacaoCommandValidator : AbstractValidator<RemoverTransacaoCommand>
{
    public RemoverTransacaoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID da transação é obrigatório.");

        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("O ID do usuário é obrigatório.");
    }
}