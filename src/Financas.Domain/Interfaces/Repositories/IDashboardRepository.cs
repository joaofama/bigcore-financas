namespace Financas.Domain.Interfaces.Repositories;

public interface IDashboardRepository
{
    /// <summary>
    /// Calcula o saldo acumulado (Receitas - Despesas) de todo o histórico antes de uma data.
    /// </summary>
    Task<decimal> ObterSaldoAteDataAsync(Guid usuarioId, DateTime dataLimite);

    /// <summary>
    /// Retorna o resumo de Receitas e Despesas totais de um mês específico.
    /// </summary>
    Task<(decimal Receitas, decimal Despesas)> ObterResumoMensalAsync(Guid usuarioId, int mes, int ano);

    /// <summary>
    /// Retorna o total de despesas agrupado por categoria pai para o gráfico de barras.
    /// </summary>
    Task<IEnumerable<(string Categoria, decimal Total)>> ObterTotalDespesasPorCategoriaAsync(Guid usuarioId, int mes, int ano);
}