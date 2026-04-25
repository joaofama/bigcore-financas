namespace Financas.Application.Responses.Categorias;

public record CategoriaResponse(
    Guid Id,
    string Nome,
    string Tipo,
    string Icone,
    Guid? CategoriaPaiId,
    List<CategoriaResponse> Subcategorias
);