namespace Financas.Domain.Interfaces.Services;

/// <summary>
/// Interface responsável por abstrair o mecanismo de notificações em tempo real.
/// Definida no Domain para que a aplicação possa disparar avisos sem conhecer a infraestrutura.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Notifica o dashboard do utilizador para que os dados sejam atualizados no front-end.
    /// </summary>
    /// <param name="usuarioId">ID do utilizador proprietário do dashboard.</param>
    Task NotificarAtualizacaoDashboard(Guid usuarioId);
}