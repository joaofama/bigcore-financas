using FluentValidation;
using Financas.Application.Commands.Categorias;
using Financas.Domain.Interfaces.Repositories;

namespace Financas.Application.Validators.Categorias;

public class RemoverCategoriaCommandValidator : AbstractValidator<RemoverCategoriaCommand>
{
    private readonly ICategoriaRepository _repository;

    public RemoverCategoriaCommandValidator(ICategoriaRepository repository)
    {
        _repository = repository;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID da categoria é obrigatório.");

        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("O ID do usuário é obrigatório.");

        RuleFor(x => x)
            .MustAsync(NaoPossuirSubcategorias)
            .WithMessage("Não é possível excluir esta categoria porque ela possui subcategorias vinculadas. Exclua ou mova as subcategorias primeiro.");
    }

    private async Task<bool> NaoPossuirSubcategorias(RemoverCategoriaCommand command, CancellationToken ct)
    {
        var todasCategorias = await _repository.ObterTodasPorUsuarioAsync(command.UsuarioId);

        bool possuiSubcategorias = todasCategorias.Any(c => c.CategoriaPaiId == command.Id);

        // Retorna TRUE (válido) se a categoria NÃO possuir subcategorias
        return !possuiSubcategorias;
    }
}