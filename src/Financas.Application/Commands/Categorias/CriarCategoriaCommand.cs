using Financas.Domain.Enums;
using MediatR;

namespace Financas.Application.Commands.Categorias;

public record CriarCategoriaCommand(
    Guid UsuarioId,
    string Nome,
    TipoTransacao Tipo,
    string Icone
) : IRequest<Guid>;