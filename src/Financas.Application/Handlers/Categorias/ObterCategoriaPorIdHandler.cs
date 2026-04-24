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

    public async Task<CategoriaResponse?> Handle(ObterCategoriaPorIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Busca no banco usando o ID da categoria E o ID do usuário (Segurança/Multitenancy)
        var categoria = await _repository.ObterPorIdAsync(request.Id, request.UsuarioId);

        // 2. Se não encontrar (ou não pertencer ao usuário), retorna null para o Controller tratar como 404
        if (categoria == null)
            return null;

        // 3. Mapeia a Entidade de Domínio para a Response (DTO)
        // Isso isola o seu banco de dados da sua API externa
        return new CategoriaResponse(
            categoria.Id,
            categoria.Nome,
            categoria.Icone,
            categoria.Tipo.ToString() // Converte o Enum (Receita/Despesa) para texto
        );
    }
}