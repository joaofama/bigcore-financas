using Financas.Domain.Entities;

namespace Financas.Domain.Interfaces.Repositories;

public interface ITransacaoRepository
{
    // Escrita
    Task AdicionarAsync(Transacao transacao);
    Task AtualizarAsync(Transacao transacao);
    Task RemoverAsync(Guid id, Guid usuarioId);

    // Leitura
    Task<Transacao?> ObterPorIdAsync(Guid id, Guid usuarioId);
    Task<IEnumerable<Transacao>> ObterPorMesEAnoAsync(Guid usuarioId, int mes, int ano);
}