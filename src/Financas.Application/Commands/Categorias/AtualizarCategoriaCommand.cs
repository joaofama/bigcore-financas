using MediatR;

namespace Financas.Application.Commands.Categorias;

public record AtualizarCategoriaCommand(
    Guid Id,
    Guid UsuarioId,
    string Nome,
    string Icone,
    string Tipo,
    Guid? CategoriaPaiId 
) : IRequest;