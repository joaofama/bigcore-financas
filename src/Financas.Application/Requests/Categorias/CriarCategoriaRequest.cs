namespace Financas.API.Requests.Categorias;

public record CriarCategoriaRequest(
    string Nome,
    string Icone,
    string Tipo,
    Guid? CategoriaPaiId = null
);