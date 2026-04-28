using Financas.Application.Queries.Transacoes;
using Financas.Application.Responses.Transacoes;
using Financas.Domain.Interfaces.Repositories;
using MediatR;

namespace Financas.Application.Handlers.Transacoes;

public class ObterTransacaoPorIdQueryHandler : IRequestHandler<ObterTransacaoPorIdQuery, TransacaoResponse?>
{
    private readonly ITransacaoRepository _transacaoRepository;

    public ObterTransacaoPorIdQueryHandler(ITransacaoRepository transacaoRepository)
    {
        _transacaoRepository = transacaoRepository;
    }

    public async Task<TransacaoResponse?> Handle(ObterTransacaoPorIdQuery request, CancellationToken ct)
    {
        var t = await _transacaoRepository.ObterPorIdAsync(request.Id, request.UsuarioId);

        if (t == null) return null;

        return new TransacaoResponse(
            t.Id,
            t.Descricao,
            t.Valor,
            t.Data,
            t.Tipo,
            t.CategoriaId,
            t.CategoriaPaiId != null ? t.CategoriaPaiNome! : t.CategoriaNome,
            t.CategoriaPaiId != null ? t.CategoriaPaiIcone! : t.CategoriaIcone,
            t.CategoriaPaiId != null ? t.CategoriaNome : null,
            t.CategoriaPaiId != null ? t.CategoriaIcone : null
        );
    }
}