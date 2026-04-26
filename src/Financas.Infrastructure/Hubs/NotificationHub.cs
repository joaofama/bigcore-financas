using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Financas.Infrastructure.Hubs;

/// <summary>
/// Hub do SignalR protegido por autenticação JWT.
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
}