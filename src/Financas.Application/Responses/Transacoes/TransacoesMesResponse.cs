namespace Financas.Application.Responses.Transacoes;

public record TransacoesMesResponse(
    decimal SaldoInicial,
    decimal TotalReceitas,
    decimal TotalDespesas,
    decimal SaldoAtual,
    IEnumerable<TransacaoResponse> Transacoes
);