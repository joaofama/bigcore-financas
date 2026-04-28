namespace Financas.Application.Responses.Transacoes;

public record TransacaoResponse(
    Guid Id,
    string? Descricao,
    decimal Valor,
    DateTime Data,
    string Tipo,
    Guid CategoriaId,
    string CategoriaPaiNome,
    string CategoriaPaiIcone,
    string? SubcategoriaNome,
    string? SubcategoriaIcone
);