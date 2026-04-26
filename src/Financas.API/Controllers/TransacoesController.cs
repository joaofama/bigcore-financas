using Financas.API.Controllers;
using Financas.Application.Commands.Transacoes;
using Financas.Application.Queries.Transacoes;
using Financas.Application.Requests.Transacoes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financas.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class TransacoesController : MainController
{
    private readonly IMediator _mediator;

    public TransacoesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Criar([FromBody] CriarTransacaoRequest request)
    {
        var command = new CriarTransacaoCommand(
            UsuarioId,
            request.Descricao,
            request.Valor,
            request.Data,
            request.Tipo,
            request.CategoriaId
        );

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObterPorId), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Atualizar(Guid id, [FromBody] AtualizarTransacaoRequest request)
    {
        var command = new AtualizarTransacaoCommand(
            id,
            UsuarioId,
            request.Descricao,
            request.Valor,
            request.Data,
            request.Tipo,
            request.CategoriaId
        );

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Remover(Guid id)
    {
        await _mediator.Send(new RemoverTransacaoCommand(id, UsuarioId));
        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> ObterPorId(Guid id)
    {
        var result = await _mediator.Send(new ObterTransacaoPorIdQuery(id, UsuarioId));
        return result != null ? Ok(result) : NotFound();
    }

    [HttpGet("mes/{mes:int}/ano/{ano:int}")]
    public async Task<ActionResult> ObterPorMesEAno(int mes, int ano)
    {
        var result = await _mediator.Send(new ObterTransacoesPorMesQuery(UsuarioId, mes, ano));
        return Ok(result);
    }
}