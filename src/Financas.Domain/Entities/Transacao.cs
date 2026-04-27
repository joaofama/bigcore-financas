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


    [BsonConstructor]
    protected Transacao() { }

    public Transacao(
        Guid usuarioId, string descricao, decimal valor, DateTime data, string tipo,
        Guid categoriaId, string categoriaNome, string categoriaIcone,
        Guid? categoriaPaiId, string? categoriaPaiNome, string? categoriaPaiIcone,
        Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
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
        string descricao, decimal valor, DateTime data, string tipo,
        Guid categoriaId, string categoriaNome, string categoriaIcone,
        Guid? categoriaPaiId, string? categoriaPaiNome, string? categoriaPaiIcone)
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