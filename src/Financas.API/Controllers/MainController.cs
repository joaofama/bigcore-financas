using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers;

[ApiController]
public abstract class MainController : ControllerBase
{
    protected Guid UsuarioId
    {
        get
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Usuário não identificado no token.");

            return Guid.Parse(userId);
        }
    }
}