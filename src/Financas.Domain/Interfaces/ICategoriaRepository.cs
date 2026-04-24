using Financas.Domain.Entities;

namespace Financas.Domain.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> ObterTodasAsync();
        Task<Categoria?> ObterPorIdAsync(Guid id);
        Task<Categoria?> ObterPorNomeAsync(string nome);
        Task AdicionarAsync(Categoria categoria);
        Task AtualizarAsync(Guid id, Categoria categoria);
        Task RemoverAsync(Guid id);
    }
}
