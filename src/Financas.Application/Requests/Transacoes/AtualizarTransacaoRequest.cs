namespace Financas.Application.Requests.Transacoes
{
    public record AtualizarTransacaoRequest(
        string? Descricao,
        decimal Valor,
        DateTime Data,
        string Tipo,
        Guid CategoriaId
    );
}
