using Financas.API.Requests.Categorias;
using Financas.Application.Commands.Categorias;
using Financas.Application.Queries.Categorias;
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

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarCategoriaRequest request)
    {
        var command = new CriarCategoriaCommand(
            UsuarioId,
            request.Nome,
            request.Tipo,
            request.Icone,
            request.CategoriaPaiId
        );

        var response = await _mediator.Send(command);

        return CreatedAtAction(nameof(ObterPorId), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarCategoriaRequest request)
    {
        var command = new AtualizarCategoriaCommand(
            id,
            UsuarioId,
            request.Nome,
            request.Icone,
            request.Tipo,
            request.CategoriaPaiId
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
}