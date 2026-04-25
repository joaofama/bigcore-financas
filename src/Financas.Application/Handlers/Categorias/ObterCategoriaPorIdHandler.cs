using Financas.Application.Queries.Categorias;
using Financas.Application.Responses.Categorias;
using Financas.Domain.Interfaces.Repositories;
using MediatR;

namespace Financas.Application.Handlers.Categorias;

public class ObterCategoriaPorIdQueryHandler : IRequestHandler<ObterCategoriaPorIdQuery, CategoriaResponse?>
{
    private readonly ICategoriaRepository _repository;

    public ObterCategoriaPorIdQueryHandler(ICategoriaRepository repository)
    {
        _repository = repository;
    }

    public async Task<CategoriaResponse?> Handle(ObterCategoriaPorIdQuery request, CancellationToken ct)
    {
        // 1. Busca a categoria principal solicitada
        var categoria = await _repository.ObterPorIdAsync(request.Id, request.UsuarioId);

        if (categoria == null) return null;

        // 2. Prepara a lista de subcategorias (vazia por padrão)
        List<CategoriaResponse> subcategorias = new();

        // 3. Se for uma categoria principal (Pai), carregamos seus filhos para enviar junto
        if (categoria.CategoriaPaiId == null)
        {
            var todas = await _repository.ObterTodasPorUsuarioAsync(request.UsuarioId);

            subcategorias = todas
                .Where(c => c.CategoriaPaiId == categoria.Id)
                .OrderBy(c => c.Nome)
                .Select(s => new CategoriaResponse(
                    s.Id,
                    s.Nome,
                    s.Tipo,  
                    s.Icone,
                    s.CategoriaPaiId,
                    new List<CategoriaResponse>() 
                ))
                .ToList();
        }

        // 4. Retorna a categoria formatada para a API
        return new CategoriaResponse(
            categoria.Id,
            categoria.Nome,
            categoria.Tipo,   
            categoria.Icone,
            categoria.CategoriaPaiId,
            subcategorias
        );
    }
}