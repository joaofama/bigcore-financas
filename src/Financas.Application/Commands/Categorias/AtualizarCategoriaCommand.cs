using MediatR;

namespace Financas.Application.Commands.Categorias;

// Note: Apenas 4 argumentos. O 'Tipo' não entra aqui.
public record AtualizarCategoriaCommand(
    Guid Id,
    string Nome,
    string Icone,
    Guid UsuarioId
) : IRequest;