using Financas.Application.Queries.Transacoes;
using Financas.Application.Responses.Transacoes;
using Financas.Domain.Interfaces.Repositories;
using MediatR;

namespace Financas.Application.Handlers.Transacoes;

public class ObterTransacoesPorMesQueryHandler : IRequestHandler<ObterTransacoesPorMesQuery, IEnumerable<TransacaoResponse>>
{
    private readonly ITransacaoRepository _transacaoRepository;

    public ObterTransacoesPorMesQueryHandler(ITransacaoRepository transacaoRepository)
    {
        _transacaoRepository = transacaoRepository;
    }

    public async Task<IEnumerable<TransacaoResponse>> Handle(ObterTransacoesPorMesQuery request, CancellationToken ct)
    {
        // Consulta no repositório já filtrando o intervalo do mês exato
        var transacoes = await _transacaoRepository.ObterPorMesEAnoAsync(request.UsuarioId, request.Mes, request.Ano);

        // Mapeamento direto e rápido para o objeto plano de resposta
        return transacoes.Select(t => new TransacaoResponse(
            t.Id,
            t.Descricao,
            t.Valor,
            t.Data,
            t.Tipo,
            t.CategoriaId,
            t.CategoriaNome,
            t.CategoriaIcone,
            t.CategoriaPaiId,
            t.CategoriaPaiNome,
            t.CategoriaPaiIcone
        ));
    }
}