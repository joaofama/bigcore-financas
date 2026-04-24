using Financas.Application.Commands.Categorias;
using Financas.Domain.Interfaces;
using MediatR;

namespace Financas.Application.Handlers.Categorias;

public class AtualizarCategoriaHandler : IRequestHandler<AtualizarCategoriaCommand>
{
    private readonly ICategoriaRepository _repository;
    public AtualizarCategoriaHandler(ICategoriaRepository repository) => _repository = repository;

    public async Task Handle(AtualizarCategoriaCommand request, CancellationToken ct)
    {
        var categoria = await _repository.ObterPorIdAsync(request.Id);
        if (categoria == null) throw new Exception("Categoria não encontrada.");

        categoria.Atualizar(request.Nome, request.Tipo, request.Icone);
        await _repository.AtualizarAsync(request.Id, categoria);
    }
}