using Financas.Application.Commands.Transacoes;
using Financas.Domain.Entities;
using Financas.Domain.Interfaces.Repositories;
using Financas.Domain.Interfaces.Services; 
using MediatR;

namespace Financas.Application.Handlers.Transacoes;

public class CriarTransacaoCommandHandler : IRequestHandler<CriarTransacaoCommand, Guid>
{
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly INotificationService _notificationService;

    public CriarTransacaoCommandHandler(
        ITransacaoRepository transacaoRepository,
        ICategoriaRepository categoriaRepository,
        INotificationService notificationService)
    {
        _transacaoRepository = transacaoRepository;
        _categoriaRepository = categoriaRepository;
        _notificationService = notificationService;
    }

    public async Task<Guid> Handle(CriarTransacaoCommand request, CancellationToken ct)
    {
        // 1. Busca a categoria para garantir que existe e obter os dados para o snapshot
        var categoria = await _categoriaRepository.ObterPorIdAsync(request.CategoriaId, request.UsuarioId);

        if (categoria == null)
            throw new KeyNotFoundException("A categoria informada não foi encontrada.");

        // 2. Busca a categoria pai (para o agrupamento do seu dashboard)
        Categoria? categoriaPai = null;
        if (categoria.CategoriaPaiId.HasValue)
        {
            categoriaPai = await _categoriaRepository.ObterPorIdAsync(categoria.CategoriaPaiId.Value, request.UsuarioId);
        }

        // 3. Cria a entidade de Transação com o Snapshot das categorias
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

        // 4. Persistência no MongoDB
        await _transacaoRepository.AdicionarAsync(transacao);

        // 5. NOTIFICAÇÃO (Abstraída) Hub do SignalR        
        await _notificationService.NotificarAtualizacaoDashboard(request.UsuarioId);

        return transacao.Id;
    }
}