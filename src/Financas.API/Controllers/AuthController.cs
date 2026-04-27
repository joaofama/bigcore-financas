using Financas.Application.Commands.Auth;
using Financas.Application.Requests.Auth; // Referência para o Request (DTO)
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.Email, request.Senha);
        var response = await _mediator.Send(command);

        return Ok(response);
    }
}