using Financas.Application.Commands.Transacoes;
using Financas.Domain.Interfaces.Repositories;
using FluentValidation;

namespace Financas.Application.Validators.Transacoes;

public class CriarTransacaoCommandValidator : AbstractValidator<CriarTransacaoCommand>
{
    private readonly ICategoriaRepository _categoriaRepository;

    public CriarTransacaoCommandValidator(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;

        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("O usuário deve estar identificado.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(100).WithMessage("A descrição deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("O valor da transação deve ser maior que zero.");

        RuleFor(x => x.Data)
            .NotEmpty().WithMessage("A data da transação é obrigatória.");

        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("O tipo de transação é obrigatório.")
            .Must(t => t == "R" || t == "D")
            .WithMessage("O tipo deve ser 'R' (Receita) ou 'D' (Despesa).");

        // Validação de existência da Categoria no Banco
        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("A categoria é obrigatória.")
            .MustAsync(async (command, categoriaId, ct) =>
            {
                // Garante que a categoria existe e pertence ao usuário que está criando a transação
                var categoria = await _categoriaRepository.ObterPorIdAsync(categoriaId, command.UsuarioId);
                return categoria != null;
            })
            .WithMessage("A categoria informada não existe ou não pertence ao usuário.");
    }
}