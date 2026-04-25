namespace Financas.Application.Responses.Dashboard;

public record DashboardResponse(
    decimal SaldoInicial,
    decimal TotalReceitas,
    decimal TotalDespesas,
    decimal SaldoAtual,
    List<DespesaPorCategoriaResponse> GraficoDespesas
);