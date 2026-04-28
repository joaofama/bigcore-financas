using Financas.Application.Responses.Transacoes;
using MediatR;

namespace Financas.Application.Commands.Transacoes;

public record CriarTransacaoCommand(
    Guid UsuarioId,
    string Descricao,
    decimal Valor,
    DateTime Data,
    string Tipo,
    Guid CategoriaId
) : IRequest<TransacaoResponse>; 