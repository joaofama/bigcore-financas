using System;
using FluentValidation;
using Financas.Application.Commands.Categorias;

namespace Financas.Application.Validations.Categorias;

public class AtualizarCategoriaCommandValidator : AbstractValidator<AtualizarCategoriaCommand>
{
    public AtualizarCategoriaCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID da categoria é obrigatório.");

        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("O ID do usuário é obrigatório.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .Length(3, 50).WithMessage("O nome deve ter entre 3 e 50 caracteres.");

        RuleFor(x => x.Icone)
            .NotEmpty().WithMessage("O ícone é obrigatório.");

        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("O tipo é obrigatório.")
            .Must(t => t.ToUpper() == "R" || t.ToUpper() == "D")
            .WithMessage("O tipo deve ser 'R' (Receita) ou 'D' (Despesa).");

        RuleFor(x => x.CategoriaPaiId)
            .NotEqual(Guid.Empty).When(x => x.CategoriaPaiId.HasValue)
            .WithMessage("O ID da categoria pai fornecido é inválido.")
            .NotEqual(x => x.Id).When(x => x.CategoriaPaiId.HasValue)
            .WithMessage("Uma categoria não pode ser pai de si mesma.");
    }
}