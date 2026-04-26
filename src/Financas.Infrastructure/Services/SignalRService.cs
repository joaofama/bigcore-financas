using Financas.Domain.Interfaces.Services;
using Financas.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Financas.Infrastructure.Services;

public class SignalRService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotificarAtualizacaoDashboard(Guid usuarioId)
    {
        // O SignalR utiliza o ClaimTypes.NameIdentifier do Token JWT para mapear o "User".
        // Enviamos uma mensagem com o nome "AtualizarDashboard" apenas para o utilizador específico.
        await _hubContext.Clients
            .User(usuarioId.ToString().ToLower())
            .SendAsync("AtualizarDashboard");
    }
}