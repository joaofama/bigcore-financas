using Financas.Domain.Enums;
using MediatR;

namespace Financas.Application.Commands.Categorias;

public record AtualizarCategoriaCommand(
    Guid Id,
    string Nome,
    TipoTransacao Tipo,
    string Icone
) : IRequest;