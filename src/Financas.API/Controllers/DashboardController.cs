using Financas.Application.Queries.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers;

[Authorize]
[Route("api/dashboard")]
public class DashboardController : MainController
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard([FromQuery] int mes, [FromQuery] int ano)
    {        
        var query = new ObterResumoDashboardQuery(mes, ano, UsuarioId);

        var response = await _mediator.Send(query);

        return Ok(response);
    }
}