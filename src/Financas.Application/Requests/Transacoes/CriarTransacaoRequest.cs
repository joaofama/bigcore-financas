namespace Financas.Application.Requests.Transacoes
{
    public record CriarTransacaoRequest(
        string? Descricao,
        decimal Valor,
        DateTime Data,
        string Tipo,
        Guid CategoriaId
    );
}
