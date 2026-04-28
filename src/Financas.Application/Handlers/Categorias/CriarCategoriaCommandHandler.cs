using Financas.Application.Commands.Categorias;
using Financas.Application.Responses.Categorias;
using Financas.Domain.Entities;
using Financas.Domain.Interfaces.Repositories;
using Financas.Domain.Interfaces.Services; // Para o ICacheService
using MediatR;

namespace Financas.Application.Handlers.Categorias;

public class CriarCategoriaCommandHandler : IRequestHandler<CriarCategoriaCommand, CategoriaResponse>
{
    private readonly ICategoriaRepository _repository;
    private readonly ICacheService _cache;

    public CriarCategoriaCommandHandler(ICategoriaRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<CategoriaResponse> Handle(CriarCategoriaCommand request, CancellationToken cancellationToken)
    {
        // 1. Instancia a Entidade
        var categoria = new Categoria(
            request.UsuarioId,
            request.Nome,
            request.Tipo,
            request.Icone,
            request.CategoriaPaiId
        );

        // 2. Persiste no MongoDB
        await _repository.AdicionarAsync(categoria);

        // 3. Invalidação do Cache
        var cacheKey = $"categorias:{request.UsuarioId}";
        await _cache.RemoverAsync(cacheKey);

        // 4. Retorna o Response completo para o Front-end
        return new CategoriaResponse(
            categoria.Id,
            categoria.Nome,
            categoria.Tipo,
            categoria.Icone,
            categoria.CategoriaPaiId,
            new List<CategoriaResponse>()
        );
    }
}