using Financas.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Financas.Domain.Entities;

public class Categoria
{
    [BsonId]
    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public TipoTransacao Tipo { get; private set; }
    public string Icone { get; private set; } = string.Empty;
    public bool Ativo { get; private set; }
    public List<Subcategoria> Subcategorias { get; private set; } = new();
    public DateTime? DataAlteracao { get; private set; }

    protected Categoria() { }

    public Categoria(
        Guid usuarioId,
        string nome,
        TipoTransacao tipo,
        string icone,
        List<Subcategoria>? subcategorias = null,
        Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        UsuarioId = usuarioId;
        Nome = nome;
        Tipo = tipo;
        Icone = icone;
        Ativo = true;

        if (subcategorias != null)
            Subcategorias = subcategorias;
    }

    public void Atualizar(string nome, TipoTransacao tipo, string icone)
    {
        Nome = nome;
        Tipo = tipo;
        Icone = icone;
        DataAlteracao = DateTime.UtcNow;
    }

    public void Inativar()
    {
        if (Subcategorias.Any(s => s.Ativo))
        {
            throw new InvalidOperationException(
                $"Não é possível inativar a categoria '{Nome}' porque ela ainda possui subcategorias ativas.");
        }

        Ativo = false;
        DataAlteracao = DateTime.UtcNow;
    }

    public void Ativar()
    {
        Ativo = true;
        DataAlteracao = DateTime.UtcNow;
    }

    public void AdicionarSubcategoria(string nome, string icone)
    {       
        if (Subcategorias.Any(s => s.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Já existe uma subcategoria com este nome.");

        Subcategorias.Add(new Subcategoria(nome, icone));
        DataAlteracao = DateTime.UtcNow;
    }

    public void AtualizarSubcategoria(Guid subId, string nome, string icone)
    {
        var sub = Subcategorias.FirstOrDefault(s => s.Id == subId);
        if (sub == null) return;

        sub.Atualizar(nome, icone);
        DataAlteracao = DateTime.UtcNow;
    }

    public void InativarSubcategoria(Guid subId)
    {
        var sub = Subcategorias.FirstOrDefault(s => s.Id == subId);
        sub?.Inativar();
        DataAlteracao = DateTime.UtcNow;
    }
}