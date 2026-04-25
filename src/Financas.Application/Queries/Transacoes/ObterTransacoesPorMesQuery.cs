using MediatR;
using Financas.Application.Responses.Transacoes;

namespace Financas.Application.Queries.Transacoes;

public record ObterTransacoesPorMesQuery(
    Guid UsuarioId,
    int Mes,
    int Ano
) : IRequest<IEnumerable<TransacaoResponse>>;