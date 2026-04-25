using Financas.Application.Commands.Transacoes;
using Financas.Domain.Entities;
using Financas.Domain.Interfaces.Repositories;
using MediatR;

namespace Financas.Application.Handlers.Transacoes;

public class CriarTransacaoCommandHandler : IRequestHandler<CriarTransacaoCommand, Guid>
{
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public CriarTransacaoCommandHandler(
        ITransacaoRepository transacaoRepository,
        ICategoriaRepository categoriaRepository)
    {
        _transacaoRepository = transacaoRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<Guid> Handle(CriarTransacaoCommand request, CancellationToken ct)
    {     
        var categoria = await _categoriaRepository.ObterPorIdAsync(request.CategoriaId, request.UsuarioId);

        Categoria? categoriaPai = null;
        if (categoria!.CategoriaPaiId.HasValue)
        {
            categoriaPai = await _categoriaRepository.ObterPorIdAsync(categoria.CategoriaPaiId.Value, request.UsuarioId);
        }

        var transacao = new Transacao(
            request.UsuarioId,
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

        await _transacaoRepository.AdicionarAsync(transacao);

        return transacao.Id;
    }
}