namespace Financas.Domain.Interfaces.Services
{
    public interface ITokenService
    {
        string GerarToken(Guid usuarioId, string email, string nome);
    }
}
