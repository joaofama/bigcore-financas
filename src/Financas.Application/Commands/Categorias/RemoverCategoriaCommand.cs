using MediatR;

namespace Financas.Application.Commands.Categorias;

public record RemoverCategoriaCommand(
    Guid Id,
    Guid UsuarioId
) : IRequest; 