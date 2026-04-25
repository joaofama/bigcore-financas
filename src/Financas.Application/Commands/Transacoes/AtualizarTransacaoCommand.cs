using MediatR;

namespace Financas.Application.Commands.Transacoes;

public record AtualizarTransacaoCommand(
    Guid Id,
    Guid UsuarioId,
    string Descricao,
    decimal Valor,
    DateTime Data,
    string Tipo, // "R" ou "D"
    Guid CategoriaId
) : IRequest;