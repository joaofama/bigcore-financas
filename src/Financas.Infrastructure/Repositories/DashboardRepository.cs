using Financas.Domain.Entities;
using Financas.Domain.Interfaces.Repositories;
using Financas.Infrastructure.Context;
using MongoDB.Driver;

namespace Financas.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly IMongoCollection<Transacao> _collection;

    public DashboardRepository(MongoDbContext context)
    {
        // Usamos a mesma coleção de transações, mas focados em agregação de leitura
        _collection = context.GetCollection<Transacao>("Transacoes");
    }

    public async Task<decimal> ObterSaldoAteDataAsync(Guid usuarioId, DateTime dataLimite)
    {
        var filter = Builders<Transacao>.Filter.And(
            Builders<Transacao>.Filter.Eq(t => t.UsuarioId, usuarioId),
            Builders<Transacao>.Filter.Lt(t => t.Data, dataLimite)
        );

        // Busca apenas os campos necessários (Valor e Tipo) para economizar banda
        var projection = Builders<Transacao>.Projection.Include(t => t.Valor).Include(t => t.Tipo);

        var transacoes = await _collection.Find(filter).Project<Transacao>(projection).ToListAsync();

        var receitas = transacoes.Where(t => t.Tipo == "R").Sum(t => t.Valor);
        var despesas = transacoes.Where(t => t.Tipo == "D").Sum(t => t.Valor);

        return receitas - despesas;
    }

    public async Task<(decimal Receitas, decimal Despesas)> ObterResumoMensalAsync(Guid usuarioId, int mes, int ano)
    {
        var inicioMes = new DateTime(ano, mes, 1);
        var fimMes = inicioMes.AddMonths(1).AddTicks(-1);

        var filter = Builders<Transacao>.Filter.And(
            Builders<Transacao>.Filter.Eq(t => t.UsuarioId, usuarioId),
            Builders<Transacao>.Filter.Gte(t => t.Data, inicioMes),
            Builders<Transacao>.Filter.Lte(t => t.Data, fimMes)
        );

        var projection = Builders<Transacao>.Projection.Include(t => t.Valor).Include(t => t.Tipo);
        var transacoes = await _collection.Find(filter).Project<Transacao>(projection).ToListAsync();

        var totalReceitas = transacoes.Where(t => t.Tipo == "R").Sum(t => t.Valor);
        var totalDespesas = transacoes.Where(t => t.Tipo == "D").Sum(t => t.Valor);

        return (totalReceitas, totalDespesas);
    }

    public async Task<IEnumerable<(string Categoria, decimal Total)>> ObterTotalDespesasPorCategoriaAsync(Guid usuarioId, int mes, int ano)
    {
        var inicioMes = new DateTime(ano, mes, 1);
        var fimMes = inicioMes.AddMonths(1).AddTicks(-1);

        // Pipeline de Agregação do MongoDB: Filtro -> Agrupamento -> Ordenação
        var pipeline = _collection.Aggregate()
            .Match(t => t.UsuarioId == usuarioId &&
                        t.Data >= inicioMes &&
                        t.Data <= fimMes &&
                        t.Tipo == "D")
            .Group(t => t.CategoriaPaiNome ?? t.CategoriaNome, // Agrupa pela categoria pai ou pela própria se for principal
                g => new
                {
                    Categoria = g.Key,
                    Total = g.Sum(t => t.Valor)
                })
            .SortByDescending(x => x.Total);

        var resultado = await pipeline.ToListAsync();

        return resultado.Select(x => (x.Categoria, x.Total));
    }
}