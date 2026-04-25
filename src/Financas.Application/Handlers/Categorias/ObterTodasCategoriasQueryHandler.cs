using Financas.Application.Queries.Categorias;
using Financas.Application.Responses.Categorias;
using Financas.Domain.Interfaces.Repositories;
using MediatR;

namespace Financas.Application.Handlers.Categorias;

public class ObterTodasCategoriasQueryHandler : IRequestHandler<ObterTodasCategoriasQuery, IEnumerable<CategoriaResponse>>
{
    private readonly ICategoriaRepository _repository;

    public ObterTodasCategoriasQueryHandler(ICategoriaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CategoriaResponse>> Handle(ObterTodasCategoriasQuery request, CancellationToken ct)
    {
        // 1. Busca todos os documentos do usuário de uma vez só (lista plana)
        var categoriasFlat = await _repository.ObterTodasPorUsuarioAsync(request.UsuarioId);

        // 2. Monta a árvore hierárquica na memória do servidor
        var arvore = categoriasFlat
            .Where(c => c.CategoriaPaiId == null) // Filtra apenas as categorias principais (Pais)
            .OrderBy(c => c.Nome)
            .Select(pai => new CategoriaResponse(
                pai.Id,
                pai.Nome,
                pai.Tipo, 
                pai.Icone,
                pai.CategoriaPaiId,
                // Busca os filhos que apontam para o ID deste pai
                Subcategorias: categoriasFlat
                    .Where(filho => filho.CategoriaPaiId == pai.Id)
                    .OrderBy(filho => filho.Nome)
                    .Select(filho => new CategoriaResponse(
                        filho.Id,
                        filho.Nome,
                        filho.Tipo, 
                        filho.Icone,
                        filho.CategoriaPaiId,
                        new List<CategoriaResponse>() // Filhos não possuem netos
                    ))
                    .ToList()
            ))
            .ToList();

        return arvore;
    }
}