using MediatR;
using Financas.Application.Responses.Transacoes;

namespace Financas.Application.Queries.Transacoes;

public record ObterTransacaoPorIdQuery(
    Guid Id,
    Guid UsuarioId
) : IRequest<TransacaoResponse?>;