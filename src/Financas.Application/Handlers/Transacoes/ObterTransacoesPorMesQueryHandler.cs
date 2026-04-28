using Financas.Application.Queries.Transacoes;
using Financas.Application.Responses.Transacoes;
using Financas.Domain.Interfaces.Repositories;
using MediatR;

namespace Financas.Application.Handlers.Transacoes;

public class ObterTransacoesPorMesQueryHandler : IRequestHandler<ObterTransacoesPorMesQuery, TransacoesMesResponse>
{
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly IDashboardRepository _dashboardRepository;

    public ObterTransacoesPorMesQueryHandler(
        ITransacaoRepository transacaoRepository,
        IDashboardRepository dashboardRepository)
    {
        _transacaoRepository = transacaoRepository;
        _dashboardRepository = dashboardRepository;
    }

    public async Task<TransacoesMesResponse> Handle(ObterTransacoesPorMesQuery request, CancellationToken ct)
    {
        // 1. Criar a data de início do mês para bater com a lógica do Dashboard
        var dataInicioMes = new DateTime(request.Ano, request.Mes, 1);

        // 2. Busca o Saldo Inicial exatamente como no Dashboard
        var saldoInicial = await _dashboardRepository.ObterSaldoAteDataAsync(request.UsuarioId, dataInicioMes);

        // 3. Busca as transações do mês
        var transacoesEntity = await _transacaoRepository.ObterPorMesEAnoAsync(request.UsuarioId, request.Mes, request.Ano);

        // 4. Mapeia para o response aplicando a regra visual do Salário/Categorias Raiz
        var transacoesResponse = transacoesEntity.Select(t => new TransacaoResponse(
            t.Id,
            t.Descricao,
            t.Valor,
            t.Data,
            t.Tipo,
            t.CategoriaId, // O ID real da subcategoria (Garante que o Editar funcione)

            // CATEGORIA PAI (A coluna principal do Grid)
            // Se tiver pai, assume o pai. Se não tiver (ex: Salário), assume a si mesma.
            t.CategoriaPaiId != null ? t.CategoriaPaiNome! : t.CategoriaNome,
            t.CategoriaPaiId != null ? t.CategoriaPaiIcone! : t.CategoriaIcone,

            // SUBCATEGORIA (A coluna secundária do Grid com a badge cinza)
            // Se for filha, envia o seu nome e ícone originais. Se for raiz, envia null.
            t.CategoriaPaiId != null ? t.CategoriaNome : null,
            t.CategoriaPaiId != null ? t.CategoriaIcone : null
        )).ToList();

        // 5. Calcula Receitas e Despesas do mês atual (LINQ em memória é mais rápido aqui)
        var totalReceitas = transacoesResponse.Where(t => t.Tipo == "R").Sum(t => t.Valor);
        var totalDespesas = transacoesResponse.Where(t => t.Tipo == "D").Sum(t => t.Valor);

        // 6. Calcula Saldo Atual
        var saldoAtual = saldoInicial + totalReceitas - totalDespesas;

        // 7. Retorna o objeto completo para o Front-end
        return new TransacoesMesResponse(
            SaldoInicial: saldoInicial,
            TotalReceitas: totalReceitas,
            TotalDespesas: totalDespesas,
            SaldoAtual: saldoAtual,
            Transacoes: transacoesResponse
        );
    }
}