using MediatR;
using Financas.Application.Responses.Categorias;

namespace Financas.Application.Queries.Categorias;

public record ObterTodasCategoriasQuery(Guid UsuarioId) : IRequest<IEnumerable<CategoriaResponse>>;