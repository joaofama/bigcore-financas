using MongoDB.Bson.Serialization.Attributes;

namespace Financas.Domain.Entities;

public class Transacao
{
    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public DateTime Data { get; private set; }
    public string Tipo { get; private set; } = string.Empty;

    // --- DADOS DA CATEGORIA (FILHA) ---
    public Guid CategoriaId { get; private set; }
    public string CategoriaNome { get; private set; } = string.Empty;
    public string CategoriaIcone { get; private set; } = string.Empty;

    // --- DADOS DA CATEGORIA PAI (OPCIONAL) ---
    public Guid? CategoriaPaiId { get; private set; }
    public string? CategoriaPaiNome { get; private set; }
    public string? CategoriaPaiIcone { get; private set; }

    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAlteracao { get; private set; }

    // Construtor utilizado pelo MongoDB para reconstruir o objeto do banco
    [BsonConstructor]
    protected Transacao() { }

    // Construtor utilizado pela aplicação para criar novas transações
    public Transacao(
        Guid usuarioId,
        string descricao,
        decimal valor,
        DateTime data,
        string tipo,
        Guid categoriaId,
        string categoriaNome,
        string categoriaIcone,
        Guid? categoriaPaiId,
        string? categoriaPaiNome,
        string? categoriaPaiIcone)
    {
        Id = Guid.NewGuid(); // O ID é gerado sempre aqui na criação
        UsuarioId = usuarioId;
        Descricao = descricao;
        Valor = Math.Round(valor, 2);
        Data = data;
        Tipo = tipo.ToUpper();

        CategoriaId = categoriaId;
        CategoriaNome = categoriaNome;
        CategoriaIcone = categoriaIcone;

        CategoriaPaiId = categoriaPaiId;
        CategoriaPaiNome = categoriaPaiNome;
        CategoriaPaiIcone = categoriaPaiIcone;

        DataCriacao = DateTime.UtcNow;
    }

    public void Atualizar(
        string descricao,
        decimal valor,
        DateTime data,
        string tipo,
        Guid categoriaId,
        string categoriaNome,
        string categoriaIcone,
        Guid? categoriaPaiId,
        string? categoriaPaiNome,
        string? categoriaPaiIcone)
    {
        Descricao = descricao;
        Valor = Math.Round(valor, 2);
        Data = data;
        Tipo = tipo.ToUpper();

        CategoriaId = categoriaId;
        CategoriaNome = categoriaNome;
        CategoriaIcone = categoriaIcone;

        CategoriaPaiId = categoriaPaiId;
        CategoriaPaiNome = categoriaPaiNome;
        CategoriaPaiIcone = categoriaPaiIcone;

        DataAlteracao = DateTime.UtcNow;
    }
}