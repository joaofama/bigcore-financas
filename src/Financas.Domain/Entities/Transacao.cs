using Financas.Domain.Enums;
using Financas.Domain.ValueObjects;

namespace Financas.Domain.Entities;

public class Transacao
{
    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public DateTime Data { get; private set; }
    public TipoTransacao Tipo { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAlteracao { get; private set; }

    public CategoriaInfo Categoria { get; private set; } = null!;
    public SubcategoriaInfo? Subcategoria { get; private set; }

    protected Transacao() { }

    public Transacao(
        Guid usuarioId,
        string descricao,
        decimal valor,
        DateTime data,
        TipoTransacao tipo,
        CategoriaInfo categoria,
        SubcategoriaInfo? subcategoria = null)
    {
        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        Descricao = descricao;
        Valor = valor;
        Data = data;
        Tipo = tipo;
        Categoria = categoria;
        Subcategoria = subcategoria;
        Ativo = true;
        DataCriacao = DateTime.UtcNow;
    }

    public void Atualizar(
        string descricao,
        decimal valor,
        DateTime data,
        CategoriaInfo categoria,
        SubcategoriaInfo? subcategoria)
    {
        Descricao = descricao;
        Valor = valor;
        Data = data;
        Categoria = categoria;
        Subcategoria = subcategoria;
        DataAlteracao = DateTime.UtcNow; // Importante para o controle do Cache no Redis
    }

    public void Inativar()
    {
        Ativo = false;
        DataAlteracao = DateTime.UtcNow;
    }
}