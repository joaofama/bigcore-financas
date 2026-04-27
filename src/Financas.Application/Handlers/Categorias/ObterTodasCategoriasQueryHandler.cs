using Financas.Application.Queries.Categorias;
using Financas.Application.Responses.Categorias;
using Financas.Domain.Interfaces.Repositories;
using Financas.Domain.Interfaces.Services;
using MediatR;

namespace Financas.Application.Handlers.Categorias;

public class ObterTodasCategoriasQueryHandler : IRequestHandler<ObterTodasCategoriasQuery, IEnumerable<CategoriaResponse>>
{
    private readonly ICategoriaRepository _repository;
    private readonly ICacheService _cache;

    public ObterTodasCategoriasQueryHandler(ICategoriaRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IEnumerable<CategoriaResponse>> Handle(ObterTodasCategoriasQuery request, CancellationToken ct)
    {
        var cacheKey = $"categorias:{request.UsuarioId}";

        // 1. Tenta buscar no Redis
        var categoriasCache = await _cache.ObterAsync<IEnumerable<CategoriaResponse>>(cacheKey);
        if (categoriasCache != null) return categoriasCache;

        // 2. Cache Miss: Busca a lista do MongoDB
        var categoriasFlat = await _repository.ObterTodasPorUsuarioAsync(request.UsuarioId);

        // 3. Monta a Hierarquia (Árvore) respeitando a ordem: Id, Nome, Tipo, Icone...
        var response = categoriasFlat
            .Where(c => c.CategoriaPaiId == null)
            .Select(pai => new CategoriaResponse(
                pai.Id,
                pai.Nome,
                pai.Tipo,  
                pai.Icone, 
                pai.CategoriaPaiId,
                categoriasFlat
                    .Where(filho => filho.CategoriaPaiId == pai.Id)
                    .Select(filho => new CategoriaResponse(
                        filho.Id,
                        filho.Nome,
                        filho.Tipo,  
                        filho.Icone, 
                        filho.CategoriaPaiId,
                        new List<CategoriaResponse>()
                    ))
                    .ToList()
            )).ToList();

        // 4. Salva no Redis a árvore completa
        await _cache.DefinirAsync(cacheKey, response, TimeSpan.FromHours(2));

        return response;
    }
}