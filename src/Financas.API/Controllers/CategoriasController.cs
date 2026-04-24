using Financas.API.Requests.Categorias;
using Financas.Application.Commands.Categorias;
using Financas.Application.Queries.Categorias;
using Financas.Application.Requests.Categorias;
using Financas.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class CategoriasController : MainController
{
    private readonly IMediator _mediator;

    public CategoriasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("receitas")]
    public async Task<IActionResult> CriarReceita([FromBody] CriarCategoriaRequest request)
    {
        // Monta o comando com 4 argumentos: Nome, Icone, Enum Tipo e Guid UsuarioId
        var command = new CriarCategoriaCommand(UsuarioId, request.Nome, TipoTransacao.Receita, request.Icone);

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObterPorId), new { id }, id);
    }

    [HttpPost("despesas")]
    public async Task<IActionResult> CriarDespesa([FromBody] CriarCategoriaRequest request)
    {
        var command = new CriarCategoriaCommand(UsuarioId, request.Nome, TipoTransacao.Despesa, request.Icone);

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(ObterPorId), new { id }, id);
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodas()
    {
        var resultado = await _mediator.Send(new ObterTodasCategoriasQuery(UsuarioId));
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var resultado = await _mediator.Send(new ObterCategoriaPorIdQuery(id, UsuarioId));

        if (resultado == null)
            return NotFound(new { mensagem = "Categoria não encontrada." });

        return Ok(resultado);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarCategoriaRequest request)
    {
        var command = new AtualizarCategoriaCommand(
            id,
            request.Nome,
            request.Icone,
            UsuarioId
        );

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        await _mediator.Send(new RemoverCategoriaCommand(id, UsuarioId));
        return NoContent();
    }
}