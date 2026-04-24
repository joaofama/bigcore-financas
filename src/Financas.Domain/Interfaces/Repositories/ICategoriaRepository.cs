using Financas.Domain.Entities;

namespace Financas.Domain.Interfaces.Repositories
{
    public interface ICategoriaRepository
    {
        // Busca apenas as categorias do usuário logado
        Task<IEnumerable<Categoria>> ObterTodasPorUsuarioAsync(Guid usuarioId);

        // Busca uma categoria específica validando o dono
        Task<Categoria?> ObterPorIdAsync(Guid id, Guid usuarioId);

        // Útil para evitar nomes duplicados dentro da conta do mesmo usuário
        Task<Categoria?> ObterPorNomeAsync(string nome, Guid usuarioId);

        // A entidade 'Categoria' já deve vir com o UsuarioId preenchido
        Task AdicionarAsync(Categoria categoria);

        // Atualiza a categoria garantindo que ela pertence ao usuário
        Task AtualizarAsync(Categoria categoria);

        // Remove apenas se o ID e o dono coincidirem
        Task RemoverAsync(Guid id, Guid usuarioId);
    }
}