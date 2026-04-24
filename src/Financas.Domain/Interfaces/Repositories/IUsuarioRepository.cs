using Financas.Domain.Entities;

namespace Financas.Domain.Interfaces.Repositories
{  
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorCredenciaisAsync(string email, string senha);
    }
}
