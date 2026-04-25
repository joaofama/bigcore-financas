namespace Financas.Application.Responses.Dashboard
{
    public record DespesaPorCategoriaResponse(
        string Categoria,
        decimal Valor
    );
}
