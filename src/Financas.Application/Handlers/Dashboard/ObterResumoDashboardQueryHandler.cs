using Financas.Application.Queries.Dashboard;
using Financas.Application.Responses.Dashboard;
using Financas.Domain.Interfaces.Repositories;
using MediatR;

namespace Financas.Application.Handlers.Dashboard;

public class ObterResumoDashboardQueryHandler : IRequestHandler<ObterResumoDashboardQuery, DashboardResponse>
{
    private readonly IDashboardRepository _dashboardRepository;

    public ObterResumoDashboardQueryHandler(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<DashboardResponse> Handle(ObterResumoDashboardQuery request, CancellationToken ct)
    {
        // 1. Define a data de corte para o Saldo Inicial (Primeiro dia do mês solicitado)
        var dataInicioMes = new DateTime(request.Ano, request.Mes, 1);

        // 2. Executa as chamadas ao repositório (Poderia ser Task.WhenAll para performance extrema, 
        // mas vamos manter sequencial para clareza inicial)

        var saldoInicial = await _dashboardRepository.ObterSaldoAteDataAsync(request.UsuarioId, dataInicioMes);

        var (receitas, despesas) = await _dashboardRepository.ObterResumoMensalAsync(request.UsuarioId, request.Mes, request.Ano);

        var graficoData = await _dashboardRepository.ObterTotalDespesasPorCategoriaAsync(request.UsuarioId, request.Mes, request.Ano);

        // 3. Cálculos Finais
        var saldoAtual = saldoInicial + receitas - despesas;

        // 4. Mapeamento para o Response
        return new DashboardResponse(
            SaldoInicial: saldoInicial,
            TotalReceitas: receitas,
            TotalDespesas: despesas,
            SaldoAtual: saldoAtual,
            GraficoDespesas: graficoData.Select(x => new DespesaPorCategoriaResponse(x.Categoria.ToUpper(), x.Total)).ToList()
        );
    }
}