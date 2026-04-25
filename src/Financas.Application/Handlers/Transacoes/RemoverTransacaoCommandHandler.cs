using Financas.Application.Commands.Transacoes;
using Financas.Domain.Interfaces.Repositories;
using MediatR;

namespace Financas.Application.Handlers.Transacoes;

public class RemoverTransacaoCommandHandler : IRequestHandler<RemoverTransacaoCommand>
{
    private readonly ITransacaoRepository _repository;

    public RemoverTransacaoCommandHandler(ITransacaoRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(RemoverTransacaoCommand request, CancellationToken ct)
    {
        await _repository.RemoverAsync(request.Id, request.UsuarioId);
    }
}