using Financas.Domain.Entities;
using Financas.Domain.Interfaces.Repositories;
using Financas.Infrastructure.Context;
using MongoDB.Driver;

namespace Financas.Infrastructure.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly IMongoCollection<Transacao> _collection;
    private readonly IMongoCollection<Usuario> _userCollection;

    public DashboardRepository(MongoDbContext context)
    {
        _collection = context.GetCollection<Transacao>("Transacoes");
        _userCollection = context.GetCollection<Usuario>("Usuarios");
    }

    public async Task<decimal> ObterSaldoAteDataAsync(Guid usuarioId, DateTime dataLimite)
    {
        // 1. Busca o Saldo Inicial que o usuário definiu no cadastro
        var userFilter = Builders<Usuario>.Filter.Eq(u => u.Id, usuarioId);
        var usuario = await _userCollection.Find(userFilter).FirstOrDefaultAsync();
        var saldoDoCadastro = usuario?.SaldoInicial ?? 0;

        // 2. Filtra as transações ocorridas ANTES da data limite (meses anteriores)
        var filter = Builders<Transacao>.Filter.And(
            Builders<Transacao>.Filter.Eq(t => t.UsuarioId, usuarioId),
            Builders<Transacao>.Filter.Lt(t => t.Data, dataLimite)
        );

        var projection = Builders<Transacao>.Projection.Include(t => t.Valor).Include(t => t.Tipo);
        var transacoes = await _collection.Find(filter).Project<Transacao>(projection).ToListAsync();

        var receitas = transacoes.Where(t => t.Tipo == "R").Sum(t => t.Valor);
        var despesas = transacoes.Where(t => t.Tipo == "D").Sum(t => t.Valor);

        // 3. O saldo inicial do mês é: Saldo do Cadastro + (Receitas - Despesas de meses anteriores)
        return saldoDoCadastro + (receitas - despesas);
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

        var pipeline = _collection.Aggregate()
            .Match(t => t.UsuarioId == usuarioId &&
                        t.Data >= inicioMes &&
                        t.Data <= fimMes &&
                        t.Tipo == "D")
            .Group(t => t.CategoriaPaiNome ?? t.CategoriaNome,
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