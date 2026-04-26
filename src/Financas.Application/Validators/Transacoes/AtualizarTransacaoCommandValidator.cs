using FluentValidation;
using Financas.Application.Commands.Transacoes;
using Financas.Domain.Interfaces.Repositories;

namespace Financas.Application.Validators.Transacoes;

public class AtualizarTransacaoCommandValidator : AbstractValidator<AtualizarTransacaoCommand>
{
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public AtualizarTransacaoCommandValidator(
        ITransacaoRepository transacaoRepository,
        ICategoriaRepository categoriaRepository)
    {
        _transacaoRepository = transacaoRepository;
        _categoriaRepository = categoriaRepository;

        // Validação de consistência de ID (Rota vs Body)
        RuleFor(x => x)
            .Must(x => x.Id == x.Id)
            .WithMessage("O ID da transação informado na URL deve ser igual ao do corpo da requisição.");

        // Validação de existência da Transação
        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(async (command, id, ct) =>
            {
                var existe = await _transacaoRepository.ObterPorIdAsync(id, command.UsuarioId);
                return existe != null;
            })
            .WithMessage("Transação não encontrada.");

        // Validação de existência da Categoria
        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("A categoria é obrigatória.")
            .MustAsync(async (command, categoriaId, ct) =>
            {
                var existe = await _categoriaRepository.ObterPorIdAsync(categoriaId, command.UsuarioId);
                return existe != null;
            })
            .WithMessage("A categoria informada não existe ou não pertence ao usuário.");

        // Outras validações
        RuleFor(x => x.Descricao).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Valor).GreaterThan(0);
        RuleFor(x => x.Tipo).Must(t => t == "R" || t == "D").WithMessage("Tipo inválido.");
    }
}