namespace Financas.Domain.Entities;

public class Subcategoria
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Icone { get; private set; } = string.Empty;
    public bool Ativo { get; private set; }

    protected Subcategoria() { }

    public Subcategoria(string nome, string icone, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        Nome = nome;
        Icone = icone;
        Ativo = true;
    }

    public void Atualizar(string nome, string icone)
    {
        Nome = nome;
        Icone = icone;
    }

    public void Inativar() => Ativo = false;
    public void Ativar() => Ativo = true;
}