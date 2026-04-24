using MediatR;
using Financas.Application.Responses.Categorias;

namespace Financas.Application.Queries.Categorias;

public record ObterCategoriaPorIdQuery(
    Guid Id,
    Guid UsuarioId
) : IRequest<CategoriaResponse?>;