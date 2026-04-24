using Financas.Domain.Entities;
using MediatR;

namespace Financas.Application.Queries.Categorias;

public record ObterCategoriaPorIdQuery(Guid Id) : IRequest<Categoria?>;