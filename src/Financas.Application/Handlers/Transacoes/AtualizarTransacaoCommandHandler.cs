using Financas.Application.Commands.Transacoes;
using Financas.Domain.Entities;
using Financas.Domain.Interfaces.Repositories;
using MediatR;

namespace Financas.Application.Handlers.Transacoes;

public class AtualizarTransacaoCommandHandler : IRequestHandler<AtualizarTransacaoCommand>
{
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public AtualizarTransacaoCommandHandler(
        ITransacaoRepository transacaoRepository,
        ICategoriaRepository categoriaRepository)
    {
        _transacaoRepository = transacaoRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task Handle(AtualizarTransacaoCommand request, CancellationToken ct)
    {
        // O Validator já garantiu que a Transação E a Categoria existem
        var transacao = await _transacaoRepository.ObterPorIdAsync(request.Id, request.UsuarioId);
        var categoria = await _categoriaRepository.ObterPorIdAsync(request.CategoriaId, request.UsuarioId);

        Categoria? categoriaPai = null;
        if (categoria!.CategoriaPaiId.HasValue)
        {
            categoriaPai = await _categoriaRepository.ObterPorIdAsync(categoria.CategoriaPaiId.Value, request.UsuarioId);
        }

        transacao!.Atualizar(
            request.Descricao,
            request.Valor,
            request.Data,
            request.Tipo,
            categoria.Id,
            categoria.Nome,
            categoria.Icone,
            categoriaPai?.Id,
            categoriaPai?.Nome,
            categoriaPai?.Icone
        );

        await _transacaoRepository.AtualizarAsync(transacao);
    }
}