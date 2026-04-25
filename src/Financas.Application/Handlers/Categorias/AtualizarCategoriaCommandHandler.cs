using Financas.Application.Commands.Categorias;
using Financas.Domain.Interfaces.Repositories;
using MediatR;

namespace Financas.Application.Handlers.Categorias;

public class AtualizarCategoriaCommandHandler : IRequestHandler<AtualizarCategoriaCommand>
{
    private readonly ICategoriaRepository _repository;

    public AtualizarCategoriaCommandHandler(ICategoriaRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(AtualizarCategoriaCommand request, CancellationToken ct)
    {
        var categoria = await _repository.ObterPorIdAsync(request.Id, request.UsuarioId);
        if (categoria == null) return;

        var tipoEnum = request.Tipo.ToUpper();

        // A entidade agora processa a atualização completa, incluindo o novo pai
        categoria.Atualizar(
            request.Nome,
            request.Icone,
            tipoEnum,
            request.CategoriaPaiId
        );

        await _repository.AtualizarAsync(categoria);
    }
}