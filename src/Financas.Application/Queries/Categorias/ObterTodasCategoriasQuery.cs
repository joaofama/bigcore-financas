using Financas.Domain.Entities;
using MediatR;

namespace Financas.Application.Queries.Categorias;

public record ObterTodasCategoriasQuery() : IRequest<IEnumerable<Categoria>>;