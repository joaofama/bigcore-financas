using Financas.Application.Commands.Categorias;
using Financas.Domain.Entities;
using Financas.Domain.Interfaces;
using MediatR;

namespace Financas.Application.Handlers.Categorias;

public class CriarCategoriaHandler : IRequestHandler<CriarCategoriaCommand, Guid>
{
    private readonly ICategoriaRepository _repository;

    public CriarCategoriaHandler(ICategoriaRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CriarCategoriaCommand request, CancellationToken cancellationToken)
    {        
        var existente = await _repository.ObterPorNomeAsync(request.Nome);
        if (existente != null) throw new Exception("Esta categoria já existe.");

        var novaCategoria = new Categoria(
            request.UsuarioId,
            request.Nome,
            request.Tipo,
            request.Icone
        );

        await _repository.AdicionarAsync(novaCategoria);

        return novaCategoria.Id;
    }
}