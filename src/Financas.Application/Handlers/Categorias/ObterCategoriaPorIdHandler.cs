using Financas.Application.Queries.Categorias;
using Financas.Domain.Entities;
using Financas.Domain.Interfaces;
using MediatR;

namespace Financas.Application.Handlers.Categorias;

public class ObterCategoriaPorIdHandler : IRequestHandler<ObterCategoriaPorIdQuery, Categoria?>
{
    private readonly ICategoriaRepository _repository;
    public ObterCategoriaPorIdHandler(ICategoriaRepository repository) => _repository = repository;

    public async Task<Categoria?> Handle(ObterCategoriaPorIdQuery request, CancellationToken ct)
        => await _repository.ObterPorIdAsync(request.Id);
}