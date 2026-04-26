using Financas.Application.Commands.Transacoes;
using Financas.Domain.Interfaces.Repositories;
using Financas.Domain.Interfaces.Services;
using MediatR;

namespace Financas.Application.Handlers.Transacoes;

public class RemoverTransacaoCommandHandler : IRequestHandler<RemoverTransacaoCommand>
{
    private readonly ITransacaoRepository _repository;
    private readonly INotificationService _notificationService; 

    public RemoverTransacaoCommandHandler(
        ITransacaoRepository repository,
        INotificationService notificationService) 
    {
        _repository = repository;
        _notificationService = notificationService;
    }

    public async Task Handle(RemoverTransacaoCommand request, CancellationToken ct)
    {
        await _repository.RemoverAsync(request.Id, request.UsuarioId);

        // Notifica que houve uma exclusão e o dashboard precisa de refresh
        await _notificationService.NotificarAtualizacaoDashboard(request.UsuarioId);
    }
}