using Financas.Application.Commands.Transacoes;
using Financas.Application.Responses.Transacoes;
using Financas.Domain.Entities;
using Financas.Domain.Interfaces.Repositories;
using Financas.Domain.Interfaces.Services;
using MediatR;

namespace Financas.Application.Handlers.Transacoes;

public class CriarTransacaoCommandHandler : IRequestHandler<CriarTransacaoCommand, TransacaoResponse>
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

    public async Task<TransacaoResponse> Handle(CriarTransacaoCommand request, CancellationToken ct)
    {
        // 1. Obtemos os dados da categoria
        var categoria = await _categoriaRepository.ObterPorIdAsync(request.CategoriaId, request.UsuarioId);

        // 2. Busca a categoria pai (caso exista) para o snapshot de agrupamento
        Categoria? categoriaPai = null;
        if (categoria!.CategoriaPaiId.HasValue)
        {
            categoriaPai = await _categoriaRepository.ObterPorIdAsync(categoria.CategoriaPaiId.Value, request.UsuarioId);
        }

        // 3. Instancia a Entidade com o Snapshot (preserva dados históricos)
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

        // 5. Notificação via SignalR para atualizar Dashboard/Gráficos
        await _notificationService.NotificarAtualizacaoDashboard(request.UsuarioId);

        // 6. Retorna a Response completa para o Controller/Frontend com a nova semântica
        return new TransacaoResponse(
            transacao.Id,
            transacao.Descricao,
            transacao.Valor,
            transacao.Data,
            transacao.Tipo,
            transacao.CategoriaId,

            // CATEGORIA PAI (Principal)
            // Se tiver pai salvo no snapshot, usa ele. Se não (como Salário), a própria categoria vira o Pai
            transacao.CategoriaPaiId != null ? transacao.CategoriaPaiNome! : transacao.CategoriaNome,
            transacao.CategoriaPaiId != null ? transacao.CategoriaPaiIcone! : transacao.CategoriaIcone,

            // SUBCATEGORIA (Detalhe)
            // Se for filha, envia o nome original dela. Se for raiz, retorna null
            transacao.CategoriaPaiId != null ? transacao.CategoriaNome : null,
            transacao.CategoriaPaiId != null ? transacao.CategoriaIcone : null
        );
    }
}