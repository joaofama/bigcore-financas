using FluentValidation;
using Financas.Application.Commands.Categorias;

namespace Financas.Application.Validators.Categorias;

public class CriarCategoriaCommandValidator : AbstractValidator<CriarCategoriaCommand>
{
    public CriarCategoriaCommandValidator()
    {
        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("O ID do usuário é obrigatório.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .Length(3, 50).WithMessage("O nome deve ter entre 3 e 50 caracteres.");

        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("O tipo (R/D) é obrigatório.")
            .Must(t => t.ToUpper() == "R" || t.ToUpper() == "D")
            .WithMessage("O tipo deve ser 'R' (Receita) ou 'D' (Despesa).");

        RuleFor(x => x.Icone)
            .NotEmpty().WithMessage("O ícone é obrigatório.");

        RuleFor(x => x.CategoriaPaiId)
            .NotEqual(Guid.Empty).When(x => x.CategoriaPaiId.HasValue)
            .WithMessage("O ID da categoria pai fornecido é inválido.");
    }
}