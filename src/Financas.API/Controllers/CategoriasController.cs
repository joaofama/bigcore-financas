using Financas.Application.Commands.Categorias;
using Financas.Application.Queries.Categorias;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Financas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Cria uma nova categoria
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarCategoriaCommand command)
    {
        var id = await _mediator.Send(command);

        // Retorna o status 201 (Created) e o local onde o recurso pode ser consultado
        return CreatedAtAction(nameof(ObterPorId), new { id }, id);
    }

    /// <summary>
    /// Lista todas as categorias cadastradas
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ObterTodas()
    {
        var query = new ObterTodasCategoriasQuery();
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    /// <summary>
    /// Obtém uma categoria específica pelo seu Guid
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var query = new ObterCategoriaPorIdQuery(id);
        var resultado = await _mediator.Send(query);

        if (resultado == null)
            return NotFound(new { mensagem = "Categoria não encontrada." });

        return Ok(resultado);
    }

    /// <summary>
    /// Atualiza os dados de uma categoria existente
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarCategoriaCommand command)
    {
        // Uma boa prática é garantir que o ID da URL seja o mesmo do comando
        if (id != command.Id)
            return BadRequest(new { mensagem = "O ID da URL não coincide com o ID do corpo da requisição." });

        await _mediator.Send(command);
        return NoContent(); // Retorna 204 sem conteúdo, padrão para atualizações com sucesso
    }

    /// <summary>
    /// Remove (ou inativa) uma categoria
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        var command = new RemoverCategoriaCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
}