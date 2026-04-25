using MongoDB.Bson.Serialization.Attributes;

namespace Financas.Domain.Entities;

public class Categoria
{
    [BsonId]
    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Tipo { get; private set; } = string.Empty; 
    public string Icone { get; private set; } = string.Empty;
    public bool Ativo { get; private set; }
    public DateTime? DataAlteracao { get; private set; }
    public Guid? CategoriaPaiId { get; private set; }

    protected Categoria() { }

    public Categoria(
        Guid usuarioId,
        string nome,
        string tipo,
        string icone,
        Guid? categoriaPaiId = null,
        Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        UsuarioId = usuarioId;
        Nome = nome;
        Tipo = tipo.ToUpper();
        Icone = icone;
        CategoriaPaiId = categoriaPaiId;
        Ativo = true;
    }

    public void Atualizar(string nome, string icone, string tipo, Guid? categoriaPaiId)
    {
        Nome = nome;
        Icone = icone;
        Tipo = tipo.ToUpper();
        CategoriaPaiId = categoriaPaiId;
        DataAlteracao = DateTime.UtcNow;
    }

    public void Inativar()
    {
        Ativo = false;
        DataAlteracao = DateTime.UtcNow;
    }

    public void Ativar()
    {
        Ativo = true;
        DataAlteracao = DateTime.UtcNow;
    }
}