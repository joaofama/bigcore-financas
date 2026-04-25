using MongoDB.Bson.Serialization.Attributes;

namespace Financas.Domain.Entities;

public class Transacao
{
    [BsonId]
    public Guid Id { get; private set; }

    public Guid UsuarioId { get; private set; }

    public string Descricao { get; private set; } = string.Empty;

    public decimal Valor { get; private set; }

    public DateTime Data { get; private set; }

    public string Tipo { get; private set; } = string.Empty; // "R" ou "D"

    public Guid CategoriaId { get; private set; }

    public bool Pago { get; private set; }

    public DateTime DataCriacao { get; private set; }

    public DateTime? DataAlteracao { get; private set; }

    protected Transacao() { }

    public Transacao(
        Guid usuarioId,
        string descricao,
        decimal valor,
        DateTime data,
        string tipo,
        Guid categoriaId,
        bool pago = true,
        Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        UsuarioId = usuarioId;
        Descricao = descricao;
        Valor = valor;
        Data = data;
        Tipo = tipo.ToUpper();
        CategoriaId = categoriaId;
        Pago = pago;
        DataCriacao = DateTime.UtcNow;
    }

    public void Atualizar(
        string descricao,
        decimal valor,
        DateTime data,
        string tipo,
        Guid categoriaId,
        bool pago)
    {
        Descricao = descricao;
        Valor = valor;
        Data = data;
        Tipo = tipo.ToUpper();
        CategoriaId = categoriaId;
        Pago = pago;
        DataAlteracao = DateTime.UtcNow;
    }

    public void AlternarStatusPagamento()
    {
        Pago = !Pago;
        DataAlteracao = DateTime.UtcNow;
    }
}