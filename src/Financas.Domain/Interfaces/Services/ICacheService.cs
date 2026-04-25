namespace Financas.Domain.Interfaces.Services;

public interface ICacheService
{
    Task<T?> ObterAsync<T>(string chave);
    Task DefinirAsync<T>(string chave, T valor, TimeSpan? expiracao = null);
    Task RemoverAsync(string chave);
}