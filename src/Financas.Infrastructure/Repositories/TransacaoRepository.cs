using Financas.Domain.Entities;
using Financas.Domain.Interfaces.Repositories;
using Financas.Infrastructure.Context;
using MongoDB.Driver;

namespace Financas.Infrastructure.Repositories;

public class TransacaoRepository : ITransacaoRepository
{
    private readonly IMongoCollection<Transacao> _transacoes;

    public TransacaoRepository(MongoDbContext context)
    {
        _transacoes = context.Transacoes;
    }

    public async Task AdicionarAsync(Transacao transacao)
    {
        await _transacoes.InsertOneAsync(transacao);
    }

    public async Task AtualizarAsync(Transacao transacao)
    {
        await _transacoes.ReplaceOneAsync(
            t => t.Id == transacao.Id && t.UsuarioId == transacao.UsuarioId,
            transacao
        );
    }

    public async Task RemoverAsync(Guid id, Guid usuarioId)
    {
        await _transacoes.DeleteOneAsync(t => t.Id == id && t.UsuarioId == usuarioId);
    }

    public async Task<Transacao?> ObterPorIdAsync(Guid id, Guid usuarioId)
    {
        return await _transacoes
            .Find(t => t.Id == id && t.UsuarioId == usuarioId)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Transacao>> ObterPorMesEAnoAsync(Guid usuarioId, int mes, int ano)
    {
        var dataInicio = new DateTime(ano, mes, 1, 0, 0, 0, DateTimeKind.Utc);
        var dataFim = dataInicio.AddMonths(1);

        return await _transacoes
            .Find(t => t.UsuarioId == usuarioId && t.Data >= dataInicio && t.Data < dataFim)
            .SortByDescending(t => t.Data)
            .ToListAsync();
    }
}