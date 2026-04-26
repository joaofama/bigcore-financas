using Financas.Application.Commands.Transacoes;
using Financas.Domain.Entities;
using Financas.Domain.Interfaces.Repositories;
using Financas.Domain.Interfaces.Services; 
using MediatR;

namespace Financas.Application.Handlers.Transacoes;

public class AtualizarTransacaoCommandHandler : IRequestHandler<AtualizarTransacaoCommand>
{
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly INotificationService _notificationService; 

    public AtualizarTransacaoCommandHandler(
        ITransacaoRepository transacaoRepository,
        ICategoriaRepository categoriaRepository,
        INotificationService notificationService) 
    {
        _transacaoRepository = transacaoRepository;
        _categoriaRepository = categoriaRepository;
        _notificationService = notificationService;
    }

    public async Task Handle(AtualizarTransacaoCommand request, CancellationToken ct)
    {        
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

        // Notifica que os dados mudaram
        await _notificationService.NotificarAtualizacaoDashboard(request.UsuarioId);
    }
}