namespace Financas.API.Requests.Categorias;

public record AtualizarCategoriaRequest(
    string Nome,
    string Icone,
    string Tipo,
    Guid? CategoriaPaiId = null
);